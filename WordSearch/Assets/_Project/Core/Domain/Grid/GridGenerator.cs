using System;
using System.Collections.Generic;

namespace RagazziStudios.Core.Domain.Grid
{
    /// <summary>
    /// Gera o grid completo do caça-palavras.
    /// Usa seed determinístico para reprodutibilidade.
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public class GridGenerator
    {
        private const string FILL_LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private readonly Random _random;

        public GridGenerator(int seed)
        {
            _random = new Random(seed);
        }

        /// <summary>
        /// Gera um grid com as palavras posicionadas e células vazias preenchidas.
        /// </summary>
        /// <param name="rows">Número de linhas.</param>
        /// <param name="cols">Número de colunas.</param>
        /// <param name="normalizedWords">Palavras normalizadas (sem acento, maiúscula).</param>
        /// <param name="displayWords">Palavras originais (com acento) na mesma ordem.</param>
        /// <returns>Tupla com o grid preenchido e a lista de posicionamentos.</returns>
        public (GridData grid, List<WordPlacement> placements) Generate(
            int rows, int cols,
            IReadOnlyList<string> normalizedWords,
            IReadOnlyList<string> displayWords)
        {
            if (normalizedWords.Count != displayWords.Count)
            {
                throw new ArgumentException(
                    "normalizedWords and displayWords must have the same count.");
            }

            var grid = new GridData(rows, cols);
            var placements = new List<WordPlacement>();

            // Ordenar palavras por tamanho (maior primeiro) para facilitar posicionamento
            var indices = CreateShuffledLengthOrder(normalizedWords);

            foreach (int idx in indices)
            {
                string normalized = normalizedWords[idx];
                string display = displayWords[idx];

                var placement = TryPlaceWord(grid, normalized, display);
                if (placement != null)
                {
                    placements.Add(placement);
                }
            }

            FillEmptyCells(grid);

            return (grid, placements);
        }

        /// <summary>
        /// Tenta posicionar uma palavra no grid em posição e direção aleatórias.
        /// Faz múltiplas tentativas antes de desistir.
        /// </summary>
        private WordPlacement TryPlaceWord(GridData grid,
            string normalizedWord, string displayWord)
        {
            var directions = (Direction[])Enum.GetValues(typeof(Direction));
            int maxAttempts = grid.Rows * grid.Cols * directions.Length;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var direction = directions[_random.Next(directions.Length)];

                var (maxRow, maxCol) = WordPlacer.GetMaxStart(
                    grid, normalizedWord.Length, direction);

                if (maxRow < 0 || maxCol < 0)
                    continue;

                int startRow = _random.Next(maxRow + 1);
                int startCol = _random.Next(maxCol + 1);

                if (WordPlacer.TryPlace(grid, normalizedWord,
                    startRow, startCol, direction))
                {
                    return new WordPlacement(
                        normalizedWord, displayWord,
                        startRow, startCol, direction);
                }
            }

            return null;
        }

        /// <summary>
        /// Preenche todas as células vazias com letras aleatórias.
        /// </summary>
        private void FillEmptyCells(GridData grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Cols; c++)
                {
                    if (grid.Cells[r, c].IsEmpty)
                    {
                        grid.SetLetter(r, c,
                            FILL_LETTERS[_random.Next(FILL_LETTERS.Length)]);
                    }
                }
            }
        }

        /// <summary>
        /// Retorna os índices ordenados por tamanho da palavra (maior primeiro),
        /// com shuffle entre palavras do mesmo tamanho.
        /// </summary>
        private int[] CreateShuffledLengthOrder(IReadOnlyList<string> words)
        {
            int[] indices = new int[words.Count];
            for (int i = 0; i < indices.Length; i++)
                indices[i] = i;

            // Ordenar por tamanho decrescente
            Array.Sort(indices, (a, b) =>
            {
                int cmp = words[b].Length.CompareTo(words[a].Length);
                if (cmp != 0) return cmp;
                // Shuffle entre palavras do mesmo tamanho
                return _random.Next(-1, 2);
            });

            return indices;
        }
    }
}
