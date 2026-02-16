namespace RagazziStudios.Core.Domain.Level
{
    /// <summary>
    /// Configuração de dificuldade de um nível.
    /// Define tamanho do grid e quantidade de palavras.
    /// </summary>
    public class DifficultyConfig
    {
        public int GridRows { get; }
        public int GridCols { get; }
        public int MinWords { get; }
        public int MaxWords { get; }

        public DifficultyConfig(int gridRows, int gridCols, int minWords, int maxWords)
        {
            GridRows = gridRows;
            GridCols = gridCols;
            MinWords = minWords;
            MaxWords = maxWords;
        }

        /// <summary>
        /// Retorna a configuração de dificuldade para um número de nível (1-15).
        /// Níveis 1-5: Fácil (8x8, 5-6 palavras)
        /// Níveis 6-10: Médio (10x10, 6-8 palavras)
        /// Níveis 11-15: Difícil (12x12, 8-10 palavras)
        /// </summary>
        public static DifficultyConfig ForLevel(int levelNumber)
        {
            if (levelNumber <= 5)
                return new DifficultyConfig(8, 8, 5, 6);
            if (levelNumber <= 10)
                return new DifficultyConfig(10, 10, 6, 8);

            return new DifficultyConfig(12, 12, 8, 10);
        }
    }
}
