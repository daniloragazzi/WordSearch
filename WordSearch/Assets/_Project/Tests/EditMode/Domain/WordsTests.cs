using System;
using System.Collections.Generic;
using NUnit.Framework;
using RagazziStudios.Core.Domain.Words;

namespace RagazziStudios.Tests.Domain.Words
{
    /// <summary>
    /// Testes unitários para TextNormalizer, WordDatabase e WordModels.
    /// </summary>
    [TestFixture]
    public class WordsTests
    {
        // ═══════════════════════════════════════════════════
        //  TextNormalizer
        // ═══════════════════════════════════════════════════

        [TestCase("coração", "CORACAO")]
        [TestCase("café", "CAFE")]
        [TestCase("ação", "ACAO")]
        [TestCase("ELEFANTE", "ELEFANTE")]
        [TestCase("água", "AGUA")]
        [TestCase("avião", "AVIAO")]
        [TestCase("pão", "PAO")]
        [TestCase("são-paulo", "SAOPAULO")]
        [TestCase("guarda-chuva", "GUARDACHUVA")]
        [TestCase("côco", "COCO")]
        public void TextNormalizer_Normalize_RemovesAccentsAndUppercases(
            string input, string expected)
        {
            string result = TextNormalizer.Normalize(input);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TextNormalizer_Normalize_EmptyString_ReturnsEmpty()
        {
            Assert.That(TextNormalizer.Normalize(""), Is.EqualTo(string.Empty));
            Assert.That(TextNormalizer.Normalize(null), Is.EqualTo(string.Empty));
        }

        [Test]
        public void TextNormalizer_Normalize_RemovesSpacesAndHyphens()
        {
            string result = TextNormalizer.Normalize("SÃO PAULO");
            Assert.That(result, Is.EqualTo("SAOPAULO"));
        }

        [Test]
        public void TextNormalizer_Normalize_RemovesApostrophes()
        {
            string result = TextNormalizer.Normalize("d'água");
            Assert.That(result, Is.EqualTo("DAGUA"));
        }

        [TestCase("coração", "CORAÇÃO")]
        [TestCase("café", "CAFÉ")]
        [TestCase("elefante", "ELEFANTE")]
        [TestCase("GATO", "GATO")]
        public void TextNormalizer_ToDisplay_KeepsAccentsUppercase(
            string input, string expected)
        {
            string result = TextNormalizer.ToDisplay(input);
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TextNormalizer_ToDisplay_EmptyString_ReturnsEmpty()
        {
            Assert.That(TextNormalizer.ToDisplay(""), Is.EqualTo(string.Empty));
            Assert.That(TextNormalizer.ToDisplay(null), Is.EqualTo(string.Empty));
        }

        // ═══════════════════════════════════════════════════
        //  WordDatabase
        // ═══════════════════════════════════════════════════

        [Test]
        public void WordDatabase_AddCategory_StoresWords()
        {
            var db = new WordDatabase();
            var words = new List<string> { "gato", "cachorro", "elefante" };

            db.AddCategory("animais", words);

            Assert.That(db.HasCategory("animais"), Is.True);
            Assert.That(db.CategoryCount, Is.EqualTo(1));
        }

        [Test]
        public void WordDatabase_AddCategory_NormalizesWords()
        {
            var db = new WordDatabase();
            var words = new List<string> { "coração", "café" };

            db.AddCategory("test", words);
            var category = db.GetCategory("test");

            Assert.That(category.NormalizedWords[0], Is.EqualTo("CORACAO"));
            Assert.That(category.NormalizedWords[1], Is.EqualTo("CAFE"));
            Assert.That(category.DisplayWords[0], Is.EqualTo("CORAÇÃO"));
            Assert.That(category.DisplayWords[1], Is.EqualTo("CAFÉ"));
        }

        [Test]
        public void WordDatabase_AddCategory_FiltersShortWords()
        {
            var db = new WordDatabase();
            var words = new List<string> { "gato", "boi", "em", "a", "pato" };

            db.AddCategory("test", words);
            var category = db.GetCategory("test");

            // "em" (2 letras) e "a" (1 letra) devem ser filtradas (< 3 após normalização)
            // "boi" tem 3 letras, deve permanecer
            Assert.That(category.WordCount, Is.EqualTo(3)); // gato, boi, pato
        }

        [Test]
        public void WordDatabase_GetCategory_NotFound_Throws()
        {
            var db = new WordDatabase();

            Assert.Throws<KeyNotFoundException>(() => db.GetCategory("inexistente"));
        }

        [Test]
        public void WordDatabase_AddCategory_EmptyId_Throws()
        {
            var db = new WordDatabase();

            Assert.Throws<ArgumentException>(
                () => db.AddCategory("", new List<string> { "gato" }));
            Assert.Throws<ArgumentException>(
                () => db.AddCategory(null, new List<string> { "gato" }));
        }

        [Test]
        public void WordDatabase_GetCategoryIds_ReturnsAll()
        {
            var db = new WordDatabase();
            db.AddCategory("animais", new List<string> { "gato" });
            db.AddCategory("esportes", new List<string> { "futebol" });

            var ids = new List<string>(db.GetCategoryIds());

            Assert.That(ids.Count, Is.EqualTo(2));
            Assert.That(ids, Does.Contain("animais"));
            Assert.That(ids, Does.Contain("esportes"));
        }

        [Test]
        public void WordDatabase_OverwriteCategory_Replaces()
        {
            var db = new WordDatabase();
            db.AddCategory("test", new List<string> { "gato" });
            db.AddCategory("test", new List<string> { "cachorro", "pato" });

            var category = db.GetCategory("test");
            Assert.That(category.WordCount, Is.EqualTo(2));
        }

        [Test]
        public void WordDatabase_WordCategory_Properties()
        {
            var db = new WordDatabase();
            db.AddCategory("animais", new List<string> { "gato", "cachorro", "leão" });

            var category = db.GetCategory("animais");

            Assert.That(category.CategoryId, Is.EqualTo("animais"));
            Assert.That(category.WordCount, Is.EqualTo(3));
            Assert.That(category.NormalizedWords.Count, Is.EqualTo(3));
            Assert.That(category.DisplayWords.Count, Is.EqualTo(3));

            // Verificar que leão normaliza corretamente
            Assert.That(category.NormalizedWords, Does.Contain("LEAO"));
            Assert.That(category.DisplayWords, Does.Contain("LEÃO"));
        }
    }
}
