using System;
using System.Collections.Generic;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Core.Domain.Level
{
    /// <summary>
    /// Gera níveis completos do caça-palavras.
    /// Combina seed determinístico + configuração de dificuldade + banco de palavras.
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public class LevelGenerator
    {
        /// <summary>
        /// Gera um nível a partir de category ID e level number.
        /// O seed é determinístico: Hash(categoryId + levelNumber).
        /// </summary>
        /// <param name="categoryId">ID da categoria (ex: "animais").</param>
        /// <param name="levelNumber">Número do nível (1-15).</param>
        /// <param name="normalizedWords">Todas as palavras normalizadas da categoria.</param>
        /// <param name="displayWords">Todas as palavras com acentos da categoria (mesma ordem).</param>
        /// <returns>LevelData com grid e placements prontos.</returns>
        public LevelData Generate(string categoryId, int levelNumber,
            IReadOnlyList<string> normalizedWords,
            IReadOnlyList<string> displayWords)
        {
            if (string.IsNullOrEmpty(categoryId))
                throw new ArgumentException("categoryId cannot be null or empty.");

            if (normalizedWords == null || normalizedWords.Count == 0)
                throw new ArgumentException("normalizedWords cannot be null or empty.");

            if (normalizedWords.Count != displayWords.Count)
                throw new ArgumentException(
                    "normalizedWords and displayWords must have the same count.");

            // 1. Seed determinístico
            int seed = GenerateSeed(categoryId, levelNumber);

            // 2. Configuração de dificuldade
            var difficulty = DifficultyConfig.ForLevel(levelNumber);

            // 3. Selecionar palavras aleatórias usando o seed
            var random = new Random(seed);
            int wordCount = random.Next(difficulty.MinWords, difficulty.MaxWords + 1);

            var (selectedNormalized, selectedDisplay) =
                SelectWords(random, normalizedWords, displayWords,
                    wordCount, difficulty.GridCols);

            // 4. Gerar grid
            var gridGen = new GridGenerator(seed);
            var (grid, placements) = gridGen.Generate(
                difficulty.GridRows, difficulty.GridCols,
                selectedNormalized, selectedDisplay);

            return new LevelData(categoryId, levelNumber, seed,
                difficulty, grid, placements);
        }

        /// <summary>
        /// Gera um seed determinístico a partir do categoryId e levelNumber.
        /// Mesmo input → mesmo seed → mesmo grid.
        /// </summary>
        public static int GenerateSeed(string categoryId, int levelNumber)
        {
            string key = $"{categoryId}_{levelNumber}";
            return key.GetHashCode();
        }

        /// <summary>
        /// Seleciona palavras aleatórias do banco, respeitando o tamanho máximo do grid.
        /// </summary>
        private (List<string> normalized, List<string> display) SelectWords(
            Random random,
            IReadOnlyList<string> allNormalized,
            IReadOnlyList<string> allDisplay,
            int count,
            int maxWordLength)
        {
            // Filtrar palavras que cabem no grid
            var validIndices = new List<int>();
            for (int i = 0; i < allNormalized.Count; i++)
            {
                if (allNormalized[i].Length <= maxWordLength &&
                    allNormalized[i].Length >= 3)
                {
                    validIndices.Add(i);
                }
            }

            // Limitar ao disponível
            count = Math.Min(count, validIndices.Count);

            // Shuffle de Fisher-Yates nos índices válidos
            for (int i = validIndices.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = validIndices[i];
                validIndices[i] = validIndices[j];
                validIndices[j] = temp;
            }

            // Pegar as primeiras 'count' palavras
            var selectedNormalized = new List<string>(count);
            var selectedDisplay = new List<string>(count);

            for (int i = 0; i < count; i++)
            {
                int idx = validIndices[i];
                selectedNormalized.Add(allNormalized[idx]);
                selectedDisplay.Add(allDisplay[idx]);
            }

            return (selectedNormalized, selectedDisplay);
        }
    }
}
