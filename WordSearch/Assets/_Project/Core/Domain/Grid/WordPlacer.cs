using System;

namespace RagazziStudios.Core.Domain.Grid
{
    /// <summary>
    /// Responsável por posicionar palavras no grid.
    /// Valida colisões e limites antes de colocar cada palavra.
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public static class WordPlacer
    {
        /// <summary>
        /// Tenta posicionar uma palavra no grid na posição e direção informadas.
        /// Retorna true se posicionou com sucesso, false se não é possível.
        /// </summary>
        public static bool TryPlace(GridData grid, string normalizedWord,
            int startRow, int startCol, Direction direction)
        {
            if (!CanPlace(grid, normalizedWord, startRow, startCol, direction))
                return false;

            Place(grid, normalizedWord, startRow, startCol, direction);
            return true;
        }

        /// <summary>
        /// Verifica se é possível posicionar a palavra sem conflitos.
        /// Permite overlap se a letra existente é igual à letra a ser colocada.
        /// </summary>
        public static bool CanPlace(GridData grid, string word,
            int startRow, int startCol, Direction direction)
        {
            if (string.IsNullOrEmpty(word))
                return false;

            GetDirectionDeltas(direction, out int dRow, out int dCol);

            for (int i = 0; i < word.Length; i++)
            {
                int row = startRow + i * dRow;
                int col = startCol + i * dCol;

                if (!grid.IsInBounds(row, col))
                    return false;

                char existing = grid.GetLetter(row, col);
                if (existing != '\0' && existing != word[i])
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Posiciona a palavra no grid (sem validação — usar CanPlace antes).
        /// </summary>
        public static void Place(GridData grid, string word,
            int startRow, int startCol, Direction direction)
        {
            GetDirectionDeltas(direction, out int dRow, out int dCol);

            for (int i = 0; i < word.Length; i++)
            {
                int row = startRow + i * dRow;
                int col = startCol + i * dCol;

                grid.SetLetter(row, col, word[i]);
                grid.Cells[row, col].IsPartOfWord = true;
            }
        }

        /// <summary>
        /// Calcula as posições possíveis para uma palavra em uma direção.
        /// Retorna o range máximo de (startRow, startCol) válidos.
        /// </summary>
        public static (int maxRow, int maxCol) GetMaxStart(GridData grid,
            int wordLength, Direction direction)
        {
            GetDirectionDeltas(direction, out int dRow, out int dCol);

            int maxRow = grid.Rows - (dRow == 0 ? 1 : wordLength * dRow);
            int maxCol = grid.Cols - (dCol == 0 ? 1 : wordLength * dCol);

            return (maxRow, maxCol);
        }

        /// <summary>
        /// Retorna os deltas de linha e coluna para cada direção.
        /// </summary>
        public static void GetDirectionDeltas(Direction direction,
            out int dRow, out int dCol)
        {
            switch (direction)
            {
                case Direction.Horizontal:
                    dRow = 0; dCol = 1;
                    break;
                case Direction.Vertical:
                    dRow = 1; dCol = 0;
                    break;
                case Direction.DiagonalDown:
                    dRow = 1; dCol = 1;
                    break;
                default:
                    throw new ArgumentException($"Unknown direction: {direction}");
            }
        }
    }
}
