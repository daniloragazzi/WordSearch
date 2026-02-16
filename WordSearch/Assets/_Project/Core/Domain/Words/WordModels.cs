using System;
using System.Collections.Generic;

namespace RagazziStudios.Core.Domain.Words
{
    /// <summary>
    /// Modelo de dados para uma categoria de palavras.
    /// </summary>
    [Serializable]
    public class CategoryData
    {
        public string id;
        public string name;
        public string icon;
    }

    /// <summary>
    /// Modelo de dados para o arquivo categories.json.
    /// </summary>
    [Serializable]
    public class CategoriesFile
    {
        public List<CategoryData> categories;
    }

    /// <summary>
    /// Modelo de dados para um arquivo de palavras (ex: animais.json).
    /// </summary>
    [Serializable]
    public class WordsFile
    {
        public string categoryId;
        public List<string> words;
    }
}
