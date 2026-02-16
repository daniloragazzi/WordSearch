using System;
using System.Collections.Generic;
using NUnit.Framework;
using RagazziStudios.Core.Domain.Level;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Tests.Domain.Level
{
    /// <summary>
    /// Testes unitários para DifficultyConfig, LevelGenerator e LevelData.
    /// </summary>
    [TestFixture]
    public class LevelTests
    {
        // ═══════════════════════════════════════════════════
        //  DifficultyConfig
        // ═══════════════════════════════════════════════════

        [TestCase(1, 8, 8, 5, 6)]
        [TestCase(3, 8, 8, 5, 6)]
        [TestCase(5, 8, 8, 5, 6)]
        [TestCase(6, 10, 10, 6, 8)]
        [TestCase(8, 10, 10, 6, 8)]
        [TestCase(10, 10, 10, 6, 8)]
        [TestCase(11, 12, 12, 8, 10)]
        [TestCase(13, 12, 12, 8, 10)]
        [TestCase(15, 12, 12, 8, 10)]
        public void DifficultyConfig_ForLevel_ReturnsCorrectConfig(
            int level, int expectedRows, int expectedCols, int expectedMin, int expectedMax)
        {
            var config = DifficultyConfig.ForLevel(level);

            Assert.That(config.GridRows, Is.EqualTo(expectedRows));
            Assert.That(config.GridCols, Is.EqualTo(expectedCols));
            Assert.That(config.MinWords, Is.EqualTo(expectedMin));
            Assert.That(config.MaxWords, Is.EqualTo(expectedMax));
        }

        [Test]
        public void DifficultyConfig_Constructor_StoresValues()
        {
            var config = new DifficultyConfig(6, 9, 3, 7);

            Assert.That(config.GridRows, Is.EqualTo(6));
            Assert.That(config.GridCols, Is.EqualTo(9));
            Assert.That(config.MinWords, Is.EqualTo(3));
            Assert.That(config.MaxWords, Is.EqualTo(7));
        }

        [Test]
        public void DifficultyConfig_LevelBoundaries_CorrectTier()
        {
            // Boundary: nível 5 é fácil, nível 6 é médio
            var easy = DifficultyConfig.ForLevel(5);
            var medium = DifficultyConfig.ForLevel(6);
            Assert.That(easy.GridRows, Is.EqualTo(8));
            Assert.That(medium.GridRows, Is.EqualTo(10));

            // Boundary: nível 10 é médio, nível 11 é difícil
            var medium2 = DifficultyConfig.ForLevel(10);
            var hard = DifficultyConfig.ForLevel(11);
            Assert.That(medium2.GridRows, Is.EqualTo(10));
            Assert.That(hard.GridRows, Is.EqualTo(12));
        }

        [Test]
        public void DifficultyConfig_VeryHighLevel_ReturnsDifficult()
        {
            // Níveis > 15 devem retornar difícil (12x12)
            var config = DifficultyConfig.ForLevel(100);
            Assert.That(config.GridRows, Is.EqualTo(12));
            Assert.That(config.GridCols, Is.EqualTo(12));
        }

        // ═══════════════════════════════════════════════════
        //  LevelGenerator — Seed Determinístico
        // ═══════════════════════════════════════════════════

        private List<string> _testNormalized;
        private List<string> _testDisplay;

        [SetUp]
        public void SetUp()
        {
            _testNormalized = new List<string>
            {
                "GATO", "CACHORRO", "ELEFANTE", "LEAO", "PATO",
                "CAVALO", "COBRA", "SAPO", "URSO", "TIGRE",
                "GIRAFA", "MACACO", "LOBO", "RAPOSA", "FOCA"
            };
            _testDisplay = new List<string>
            {
                "GATO", "CACHORRO", "ELEFANTE", "LEÃO", "PATO",
                "CAVALO", "COBRA", "SAPO", "URSO", "TIGRE",
                "GIRAFA", "MACACO", "LOBO", "RAPOSA", "FOCA"
            };
        }

        [Test]
        public void LevelGenerator_SameSeed_SameGrid()
        {
            var gen = new LevelGenerator();

            var level1 = gen.Generate("animais", 1, _testNormalized, _testDisplay);
            var level2 = gen.Generate("animais", 1, _testNormalized, _testDisplay);

            Assert.That(level1.Grid.ToString(), Is.EqualTo(level2.Grid.ToString()),
                "Same category+level should produce identical grids.");
        }

        [Test]
        public void LevelGenerator_DifferentLevel_DifferentGrid()
        {
            var gen = new LevelGenerator();

            var level1 = gen.Generate("animais", 1, _testNormalized, _testDisplay);
            var level5 = gen.Generate("animais", 5, _testNormalized, _testDisplay);

            Assert.That(level1.Grid.ToString(), Is.Not.EqualTo(level5.Grid.ToString()),
                "Different level numbers should produce different grids.");
        }

        [Test]
        public void LevelGenerator_DifferentCategory_DifferentGrid()
        {
            var gen = new LevelGenerator();

            var levelA = gen.Generate("animais", 1, _testNormalized, _testDisplay);
            var levelB = gen.Generate("esportes", 1, _testNormalized, _testDisplay);

            Assert.That(levelA.Grid.ToString(), Is.Not.EqualTo(levelB.Grid.ToString()),
                "Different categories should produce different grids.");
        }

        [Test]
        public void LevelGenerator_GenerateSeed_Deterministic()
        {
            int seed1 = LevelGenerator.GenerateSeed("animais", 5);
            int seed2 = LevelGenerator.GenerateSeed("animais", 5);

            Assert.That(seed1, Is.EqualTo(seed2));
        }

        [Test]
        public void LevelGenerator_GenerateSeed_DifferentInputs_DifferentSeeds()
        {
            int seed1 = LevelGenerator.GenerateSeed("animais", 1);
            int seed2 = LevelGenerator.GenerateSeed("animais", 2);
            int seed3 = LevelGenerator.GenerateSeed("esportes", 1);

            Assert.That(seed1, Is.Not.EqualTo(seed2));
            Assert.That(seed1, Is.Not.EqualTo(seed3));
        }

        // ═══════════════════════════════════════════════════
        //  LevelGenerator — Nível Gerado
        // ═══════════════════════════════════════════════════

        [Test]
        public void LevelGenerator_Generate_ReturnsValidLevelData()
        {
            var gen = new LevelGenerator();
            var level = gen.Generate("animais", 1, _testNormalized, _testDisplay);

            Assert.That(level, Is.Not.Null);
            Assert.That(level.CategoryId, Is.EqualTo("animais"));
            Assert.That(level.LevelNumber, Is.EqualTo(1));
            Assert.That(level.Grid, Is.Not.Null);
            Assert.That(level.Placements, Is.Not.Null);
            Assert.That(level.Difficulty, Is.Not.Null);
        }

        [Test]
        public void LevelGenerator_Generate_GridFilled()
        {
            var gen = new LevelGenerator();
            var level = gen.Generate("animais", 1, _testNormalized, _testDisplay);

            Assert.That(level.Grid.IsFilled(), Is.True,
                "Generated grid should have all cells filled.");
        }

        [Test]
        public void LevelGenerator_Generate_CorrectGridSize()
        {
            var gen = new LevelGenerator();

            // Nível 1 = fácil = 8x8
            var level1 = gen.Generate("animais", 1, _testNormalized, _testDisplay);
            Assert.That(level1.Grid.Rows, Is.EqualTo(8));
            Assert.That(level1.Grid.Cols, Is.EqualTo(8));

            // Nível 7 = médio = 10x10
            var level7 = gen.Generate("animais", 7, _testNormalized, _testDisplay);
            Assert.That(level7.Grid.Rows, Is.EqualTo(10));
            Assert.That(level7.Grid.Cols, Is.EqualTo(10));

            // Nível 12 = difícil = 12x12
            var level12 = gen.Generate("animais", 12, _testNormalized, _testDisplay);
            Assert.That(level12.Grid.Rows, Is.EqualTo(12));
            Assert.That(level12.Grid.Cols, Is.EqualTo(12));
        }

        [Test]
        public void LevelGenerator_Generate_WordCountInRange()
        {
            var gen = new LevelGenerator();

            // Testar vários níveis
            for (int lvl = 1; lvl <= 15; lvl++)
            {
                var level = gen.Generate("animais", lvl, _testNormalized, _testDisplay);
                var difficulty = DifficultyConfig.ForLevel(lvl);

                Assert.That(level.Placements.Count,
                    Is.GreaterThanOrEqualTo(1),
                    $"Level {lvl}: should have at least 1 word placed.");

                Assert.That(level.Placements.Count,
                    Is.LessThanOrEqualTo(difficulty.MaxWords),
                    $"Level {lvl}: should not exceed max words ({difficulty.MaxWords}).");
            }
        }

        [Test]
        public void LevelGenerator_Generate_PlacedWordsInGrid()
        {
            var gen = new LevelGenerator();
            var level = gen.Generate("animais", 3, _testNormalized, _testDisplay);

            foreach (var placement in level.Placements)
            {
                var positions = placement.GetCellPositions();
                string wordInGrid = "";

                foreach (var (row, col) in positions)
                {
                    Assert.That(level.Grid.IsInBounds(row, col), Is.True,
                        $"Position [{row},{col}] should be in bounds.");
                    wordInGrid += level.Grid.GetLetter(row, col);
                }

                Assert.That(wordInGrid, Is.EqualTo(placement.NormalizedWord),
                    $"Word '{placement.NormalizedWord}' should exist at its placement positions.");
            }
        }

        [Test]
        public void LevelGenerator_Generate_WordsNotTooLong()
        {
            var gen = new LevelGenerator();
            var level = gen.Generate("animais", 1, _testNormalized, _testDisplay);

            foreach (var placement in level.Placements)
            {
                Assert.That(placement.NormalizedWord.Length,
                    Is.LessThanOrEqualTo(level.Difficulty.GridCols),
                    $"Word '{placement.NormalizedWord}' should fit in grid width.");
            }
        }

        // ═══════════════════════════════════════════════════
        //  LevelGenerator — Validações
        // ═══════════════════════════════════════════════════

        [Test]
        public void LevelGenerator_NullCategoryId_Throws()
        {
            var gen = new LevelGenerator();

            Assert.Throws<ArgumentException>(
                () => gen.Generate(null, 1, _testNormalized, _testDisplay));
        }

        [Test]
        public void LevelGenerator_EmptyWords_Throws()
        {
            var gen = new LevelGenerator();

            Assert.Throws<ArgumentException>(
                () => gen.Generate("animais", 1,
                    new List<string>(), new List<string>()));
        }

        [Test]
        public void LevelGenerator_MismatchedLists_Throws()
        {
            var gen = new LevelGenerator();

            Assert.Throws<ArgumentException>(
                () => gen.Generate("animais", 1,
                    new List<string> { "A", "B" },
                    new List<string> { "A" }));
        }

        // ═══════════════════════════════════════════════════
        //  LevelData
        // ═══════════════════════════════════════════════════

        [Test]
        public void LevelData_Constructor_StoresAllProperties()
        {
            var grid = new GridData(8, 8);
            var placements = new List<WordPlacement>();
            var difficulty = new DifficultyConfig(8, 8, 5, 6);

            var level = new LevelData("animais", 3, 12345, difficulty, grid, placements);

            Assert.That(level.CategoryId, Is.EqualTo("animais"));
            Assert.That(level.LevelNumber, Is.EqualTo(3));
            Assert.That(level.Seed, Is.EqualTo(12345));
            Assert.That(level.Difficulty, Is.SameAs(difficulty));
            Assert.That(level.Grid, Is.SameAs(grid));
            Assert.That(level.Placements, Is.SameAs(placements));
        }
    }
}
