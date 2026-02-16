using System;
using System.Text;

namespace RagazziStudios.Core.Domain.Grid
{
    /// <summary>
    /// Modelo do grid de caça-palavras.
    /// Contém a matriz de células e métodos de acesso.
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public class GridData
    {
        public int Rows { get; }
        public int Cols { get; }
        public CellData[,] Cells { get; }

        public GridData(int rows, int cols)
        {
            if (rows <= 0 || cols <= 0)
            {
                throw new ArgumentException(
                    $"Grid dimensions must be positive. Got {rows}x{cols}.");
            }

            Rows = rows;
            Cols = cols;
            Cells = new CellData[rows, cols];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Cells[r, c] = new CellData(r, c);
                }
            }
        }

        /// <summary>
        /// Acessa uma célula por posição.
        /// </summary>
        public CellData this[int row, int col] => Cells[row, col];

        /// <summary>
        /// Verifica se a posição está dentro dos limites do grid.
        /// </summary>
        public bool IsInBounds(int row, int col) =>
            row >= 0 && row < Rows && col >= 0 && col < Cols;

        /// <summary>
        /// Retorna a letra na posição informada, ou '\0' se fora dos limites.
        /// </summary>
        public char GetLetter(int row, int col) =>
            IsInBounds(row, col) ? Cells[row, col].Letter : '\0';

        /// <summary>
        /// Define a letra em uma posição.
        /// </summary>
        public void SetLetter(int row, int col, char letter)
        {
            if (!IsInBounds(row, col))
            {
                throw new ArgumentOutOfRangeException(
                    $"Position [{row},{col}] is out of bounds for {Rows}x{Cols} grid.");
            }

            Cells[row, col].Letter = letter;
        }

        /// <summary>
        /// Verifica se todas as células estão preenchidas.
        /// </summary>
        public bool IsFilled()
        {
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c].IsEmpty)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Conta quantas células estão vazias.
        /// </summary>
        public int CountEmpty()
        {
            int count = 0;
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    if (Cells[r, c].IsEmpty)
                        count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Representação textual do grid (para debug/testes).
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int r = 0; r < Rows; r++)
            {
                for (int c = 0; c < Cols; c++)
                {
                    char letter = Cells[r, c].Letter;
                    sb.Append(letter == '\0' ? '.' : letter);
                    if (c < Cols - 1) sb.Append(' ');
                }
                if (r < Rows - 1) sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
