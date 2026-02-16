using System;
using System.Collections.Generic;

namespace RagazziStudios.Core.Domain.Words
{
    /// <summary>
    /// Banco de palavras em memória para uma categoria.
    /// Armazena versões normalizada (grid) e display (lista).
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public class WordDatabase
    {
        private readonly Dictionary<string, WordCategory> _categories;

        public WordDatabase()
        {
            _categories = new Dictionary<string, WordCategory>();
        }

        /// <summary>
        /// Adiciona uma categoria com suas palavras ao banco.
        /// </summary>
        /// <param name="categoryId">ID da categoria (ex: "animais").</param>
        /// <param name="words">Palavras originais (com acento) da categoria.</param>
        public void AddCategory(string categoryId, IReadOnlyList<string> words)
        {
            if (string.IsNullOrEmpty(categoryId))
                throw new ArgumentException("categoryId cannot be null or empty.");

            var normalized = new List<string>(words.Count);
            var display = new List<string>(words.Count);

            foreach (string word in words)
            {
                string norm = TextNormalizer.Normalize(word);
                string disp = TextNormalizer.ToDisplay(word);

                // Ignorar palavras com menos de 3 letras após normalização
                if (norm.Length < 3)
                    continue;

                normalized.Add(norm);
                display.Add(disp);
            }

            _categories[categoryId] = new WordCategory(categoryId, normalized, display);
        }

        /// <summary>
        /// Retorna os dados de uma categoria.
        /// </summary>
        public WordCategory GetCategory(string categoryId)
        {
            if (_categories.TryGetValue(categoryId, out var category))
                return category;

            throw new KeyNotFoundException(
                $"Category '{categoryId}' not found in WordDatabase.");
        }

        /// <summary>
        /// Verifica se a categoria existe no banco.
        /// </summary>
        public bool HasCategory(string categoryId) =>
            _categories.ContainsKey(categoryId);

        /// <summary>
        /// Retorna todos os IDs de categorias carregadas.
        /// </summary>
        public IEnumerable<string> GetCategoryIds() => _categories.Keys;

        /// <summary>
        /// Quantidade de categorias carregadas.
        /// </summary>
        public int CategoryCount => _categories.Count;
    }

    /// <summary>
    /// Dados de uma categoria: palavras normalizadas e display.
    /// </summary>
    public class WordCategory
    {
        public string CategoryId { get; }
        public IReadOnlyList<string> NormalizedWords { get; }
        public IReadOnlyList<string> DisplayWords { get; }
        public int WordCount => NormalizedWords.Count;

        public WordCategory(string categoryId,
            List<string> normalizedWords, List<string> displayWords)
        {
            CategoryId = categoryId;
            NormalizedWords = normalizedWords.AsReadOnly();
            DisplayWords = displayWords.AsReadOnly();
        }
    }
}
