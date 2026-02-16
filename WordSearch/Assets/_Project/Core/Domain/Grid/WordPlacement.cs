namespace RagazziStudios.Core.Domain.Grid
{
    /// <summary>
    /// Representa uma palavra posicionada no grid.
    /// Guarda a posição, direção e a palavra original (com acentos).
    /// </summary>
    public class WordPlacement
    {
        /// <summary>Palavra normalizada (sem acentos, maiúscula) — usada no grid.</summary>
        public string NormalizedWord { get; }

        /// <summary>Palavra original (com acentos) — exibida na lista.</summary>
        public string DisplayWord { get; }

        public int StartRow { get; }
        public int StartCol { get; }
        public Direction Direction { get; }

        /// <summary>Se a palavra já foi encontrada pelo jogador.</summary>
        public bool Found { get; set; }

        public WordPlacement(string normalizedWord, string displayWord,
            int startRow, int startCol, Direction direction)
        {
            NormalizedWord = normalizedWord;
            DisplayWord = displayWord;
            StartRow = startRow;
            StartCol = startCol;
            Direction = direction;
            Found = false;
        }

        /// <summary>Tamanho da palavra.</summary>
        public int Length => NormalizedWord.Length;

        /// <summary>
        /// Retorna a posição (row, col) de cada letra da palavra no grid.
        /// </summary>
        public (int row, int col)[] GetCellPositions()
        {
            var positions = new (int row, int col)[Length];
            int dRow = 0, dCol = 0;

            switch (Direction)
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
            }

            for (int i = 0; i < Length; i++)
            {
                positions[i] = (StartRow + i * dRow, StartCol + i * dCol);
            }

            return positions;
        }

        public override string ToString() =>
            $"{DisplayWord} ({NormalizedWord}) at [{StartRow},{StartCol}] {Direction}";
    }
}
