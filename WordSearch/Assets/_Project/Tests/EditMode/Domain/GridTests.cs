using System;
using System.Collections.Generic;
using NUnit.Framework;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Tests.Domain.Grid
{
    /// <summary>
    /// Testes unitários para GridData, CellData, WordPlacer, GridGenerator e WordFinder.
    /// </summary>
    [TestFixture]
    public class GridTests
    {
        // ═══════════════════════════════════════════════════
        //  CellData
        // ═══════════════════════════════════════════════════

        [Test]
        public void CellData_NewCell_IsEmpty()
        {
            var cell = new CellData(0, 0);

            Assert.That(cell.IsEmpty, Is.True);
            Assert.That(cell.Letter, Is.EqualTo('\0'));
            Assert.That(cell.IsPartOfWord, Is.False);
            Assert.That(cell.Row, Is.EqualTo(0));
            Assert.That(cell.Col, Is.EqualTo(0));
        }

        [Test]
        public void CellData_SetLetter_NotEmpty()
        {
            var cell = new CellData(3, 5);
            cell.Letter = 'A';

            Assert.That(cell.IsEmpty, Is.False);
            Assert.That(cell.Letter, Is.EqualTo('A'));
        }

        // ═══════════════════════════════════════════════════
        //  GridData
        // ═══════════════════════════════════════════════════

        [Test]
        public void GridData_Constructor_CreatesCorrectSize()
        {
            var grid = new GridData(8, 10);

            Assert.That(grid.Rows, Is.EqualTo(8));
            Assert.That(grid.Cols, Is.EqualTo(10));
            Assert.That(grid.Cells.GetLength(0), Is.EqualTo(8));
            Assert.That(grid.Cells.GetLength(1), Is.EqualTo(10));
        }

        [Test]
        public void GridData_Constructor_AllCellsEmpty()
        {
            var grid = new GridData(5, 5);

            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    Assert.That(grid[r, c].IsEmpty, Is.True);

            Assert.That(grid.CountEmpty(), Is.EqualTo(25));
            Assert.That(grid.IsFilled(), Is.False);
        }

        [TestCase(0, 5)]
        [TestCase(5, 0)]
        [TestCase(-1, 5)]
        [TestCase(5, -1)]
        public void GridData_Constructor_InvalidDimensions_Throws(int rows, int cols)
        {
            Assert.Throws<ArgumentException>(() => new GridData(rows, cols));
        }

        [Test]
        public void GridData_SetLetter_SetsCorrectly()
        {
            var grid = new GridData(8, 8);
            grid.SetLetter(3, 4, 'X');

            Assert.That(grid.GetLetter(3, 4), Is.EqualTo('X'));
            Assert.That(grid[3, 4].Letter, Is.EqualTo('X'));
        }

        [Test]
        public void GridData_SetLetter_OutOfBounds_Throws()
        {
            var grid = new GridData(5, 5);

            Assert.Throws<ArgumentOutOfRangeException>(
                () => grid.SetLetter(10, 0, 'A'));
        }

        [Test]
        public void GridData_GetLetter_OutOfBounds_ReturnsNull()
        {
            var grid = new GridData(5, 5);

            Assert.That(grid.GetLetter(-1, 0), Is.EqualTo('\0'));
            Assert.That(grid.GetLetter(0, 10), Is.EqualTo('\0'));
        }

        [Test]
        public void GridData_IsInBounds_ValidPositions()
        {
            var grid = new GridData(8, 10);

            Assert.That(grid.IsInBounds(0, 0), Is.True);
            Assert.That(grid.IsInBounds(7, 9), Is.True);
            Assert.That(grid.IsInBounds(8, 0), Is.False);
            Assert.That(grid.IsInBounds(0, 10), Is.False);
            Assert.That(grid.IsInBounds(-1, 0), Is.False);
        }

        [Test]
        public void GridData_IsFilled_AllFilled()
        {
            var grid = new GridData(3, 3);
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    grid.SetLetter(r, c, 'A');

            Assert.That(grid.IsFilled(), Is.True);
            Assert.That(grid.CountEmpty(), Is.EqualTo(0));
        }

        [Test]
        public void GridData_ToString_FormatsCorrectly()
        {
            var grid = new GridData(2, 3);
            grid.SetLetter(0, 0, 'A');
            grid.SetLetter(0, 1, 'B');
            grid.SetLetter(0, 2, 'C');
            // Row 1 empty

            string result = grid.ToString();
            Assert.That(result, Does.Contain("A B C"));
            Assert.That(result, Does.Contain(". . ."));
        }

        // ═══════════════════════════════════════════════════
        //  Direction
        // ═══════════════════════════════════════════════════

        [Test]
        public void Direction_HasThreeValues()
        {
            var values = (Direction[])Enum.GetValues(typeof(Direction));
            Assert.That(values.Length, Is.EqualTo(3));
        }

        // ═══════════════════════════════════════════════════
        //  WordPlacer
        // ═══════════════════════════════════════════════════

        [Test]
        public void WordPlacer_PlaceHorizontal_Success()
        {
            var grid = new GridData(8, 8);
            bool placed = WordPlacer.TryPlace(grid, "GATO", 0, 0, Direction.Horizontal);

            Assert.That(placed, Is.True);
            Assert.That(grid.GetLetter(0, 0), Is.EqualTo('G'));
            Assert.That(grid.GetLetter(0, 1), Is.EqualTo('A'));
            Assert.That(grid.GetLetter(0, 2), Is.EqualTo('T'));
            Assert.That(grid.GetLetter(0, 3), Is.EqualTo('O'));
        }

        [Test]
        public void WordPlacer_PlaceVertical_Success()
        {
            var grid = new GridData(8, 8);
            bool placed = WordPlacer.TryPlace(grid, "GATO", 0, 0, Direction.Vertical);

            Assert.That(placed, Is.True);
            Assert.That(grid.GetLetter(0, 0), Is.EqualTo('G'));
            Assert.That(grid.GetLetter(1, 0), Is.EqualTo('A'));
            Assert.That(grid.GetLetter(2, 0), Is.EqualTo('T'));
            Assert.That(grid.GetLetter(3, 0), Is.EqualTo('O'));
        }

        [Test]
        public void WordPlacer_PlaceDiagonal_Success()
        {
            var grid = new GridData(8, 8);
            bool placed = WordPlacer.TryPlace(grid, "GATO", 0, 0, Direction.DiagonalDown);

            Assert.That(placed, Is.True);
            Assert.That(grid.GetLetter(0, 0), Is.EqualTo('G'));
            Assert.That(grid.GetLetter(1, 1), Is.EqualTo('A'));
            Assert.That(grid.GetLetter(2, 2), Is.EqualTo('T'));
            Assert.That(grid.GetLetter(3, 3), Is.EqualTo('O'));
        }

        [Test]
        public void WordPlacer_PlaceOutOfBounds_Fails()
        {
            var grid = new GridData(8, 8);

            // "ELEFANTE" (8 letras) em col 1 horizontal = precisaria col 1-8, mas max é 7
            bool placed = WordPlacer.TryPlace(grid, "ELEFANTE", 0, 1, Direction.Horizontal);

            Assert.That(placed, Is.False);
        }

        [Test]
        public void WordPlacer_PlaceOverlap_SameLetter_Succeeds()
        {
            var grid = new GridData(8, 8);

            // "GATO" horizontal em (0,0)
            WordPlacer.TryPlace(grid, "GATO", 0, 0, Direction.Horizontal);

            // "GALO" vertical em (0,0) — compartilha 'G' na posição (0,0)
            bool placed = WordPlacer.TryPlace(grid, "GALO", 0, 0, Direction.Vertical);

            Assert.That(placed, Is.True);
            Assert.That(grid.GetLetter(0, 0), Is.EqualTo('G'));
        }

        [Test]
        public void WordPlacer_PlaceOverlap_DifferentLetter_Fails()
        {
            var grid = new GridData(8, 8);

            // "GATO" horizontal em (0,0)
            WordPlacer.TryPlace(grid, "GATO", 0, 0, Direction.Horizontal);

            // "PATO" horizontal em (0,0) — 'P' conflita com 'G' existente
            bool placed = WordPlacer.TryPlace(grid, "PATO", 0, 0, Direction.Horizontal);

            Assert.That(placed, Is.False);
        }

        [Test]
        public void WordPlacer_CanPlace_EmptyWord_ReturnsFalse()
        {
            var grid = new GridData(8, 8);

            Assert.That(WordPlacer.CanPlace(grid, "", 0, 0, Direction.Horizontal), Is.False);
            Assert.That(WordPlacer.CanPlace(grid, null, 0, 0, Direction.Horizontal), Is.False);
        }

        [Test]
        public void WordPlacer_GetMaxStart_Horizontal()
        {
            var grid = new GridData(8, 8);
            var (maxRow, maxCol) = WordPlacer.GetMaxStart(grid, 5, Direction.Horizontal);

            Assert.That(maxRow, Is.EqualTo(7)); // Pode começar em qualquer row
            Assert.That(maxCol, Is.EqualTo(3)); // 8 - 5 = 3
        }

        [Test]
        public void WordPlacer_GetMaxStart_Vertical()
        {
            var grid = new GridData(8, 8);
            var (maxRow, maxCol) = WordPlacer.GetMaxStart(grid, 5, Direction.Vertical);

            Assert.That(maxRow, Is.EqualTo(3)); // 8 - 5 = 3
            Assert.That(maxCol, Is.EqualTo(7)); // Pode começar em qualquer col
        }

        [Test]
        public void WordPlacer_GetDirectionDeltas_AllDirections()
        {
            WordPlacer.GetDirectionDeltas(Direction.Horizontal, out int dR, out int dC);
            Assert.That(dR, Is.EqualTo(0));
            Assert.That(dC, Is.EqualTo(1));

            WordPlacer.GetDirectionDeltas(Direction.Vertical, out dR, out dC);
            Assert.That(dR, Is.EqualTo(1));
            Assert.That(dC, Is.EqualTo(0));

            WordPlacer.GetDirectionDeltas(Direction.DiagonalDown, out dR, out dC);
            Assert.That(dR, Is.EqualTo(1));
            Assert.That(dC, Is.EqualTo(1));
        }

        [Test]
        public void WordPlacer_Place_MarksIsPartOfWord()
        {
            var grid = new GridData(8, 8);
            WordPlacer.Place(grid, "GATO", 0, 0, Direction.Horizontal);

            Assert.That(grid[0, 0].IsPartOfWord, Is.True);
            Assert.That(grid[0, 1].IsPartOfWord, Is.True);
            Assert.That(grid[0, 4].IsPartOfWord, Is.False);
        }

        // ═══════════════════════════════════════════════════
        //  WordPlacement
        // ═══════════════════════════════════════════════════

        [Test]
        public void WordPlacement_GetCellPositions_Horizontal()
        {
            var wp = new WordPlacement("GATO", "GATO", 2, 3, Direction.Horizontal);
            var positions = wp.GetCellPositions();

            Assert.That(positions.Length, Is.EqualTo(4));
            Assert.That(positions[0], Is.EqualTo((2, 3)));
            Assert.That(positions[1], Is.EqualTo((2, 4)));
            Assert.That(positions[2], Is.EqualTo((2, 5)));
            Assert.That(positions[3], Is.EqualTo((2, 6)));
        }

        [Test]
        public void WordPlacement_GetCellPositions_Vertical()
        {
            var wp = new WordPlacement("ABC", "ABC", 1, 0, Direction.Vertical);
            var positions = wp.GetCellPositions();

            Assert.That(positions[0], Is.EqualTo((1, 0)));
            Assert.That(positions[1], Is.EqualTo((2, 0)));
            Assert.That(positions[2], Is.EqualTo((3, 0)));
        }

        [Test]
        public void WordPlacement_GetCellPositions_Diagonal()
        {
            var wp = new WordPlacement("AB", "AB", 0, 0, Direction.DiagonalDown);
            var positions = wp.GetCellPositions();

            Assert.That(positions[0], Is.EqualTo((0, 0)));
            Assert.That(positions[1], Is.EqualTo((1, 1)));
        }

        [Test]
        public void WordPlacement_Properties_Correct()
        {
            var wp = new WordPlacement("CORACAO", "CORAÇÃO", 0, 0, Direction.Horizontal);

            Assert.That(wp.NormalizedWord, Is.EqualTo("CORACAO"));
            Assert.That(wp.DisplayWord, Is.EqualTo("CORAÇÃO"));
            Assert.That(wp.Length, Is.EqualTo(7));
            Assert.That(wp.Found, Is.False);
        }

        // ═══════════════════════════════════════════════════
        //  GridGenerator
        // ═══════════════════════════════════════════════════

        [Test]
        public void GridGenerator_Generate_AllCellsFilled()
        {
            var gen = new GridGenerator(42);
            var normalized = new List<string> { "GATO", "CACHORRO", "LEAO" };
            var display = new List<string> { "GATO", "CACHORRO", "LEÃO" };

            var (grid, placements) = gen.Generate(8, 8, normalized, display);

            Assert.That(grid.IsFilled(), Is.True);
            Assert.That(grid.CountEmpty(), Is.EqualTo(0));
        }

        [Test]
        public void GridGenerator_Generate_PlacesWords()
        {
            var gen = new GridGenerator(42);
            var normalized = new List<string> { "GATO", "PATO", "LEAO" };
            var display = new List<string> { "GATO", "PATO", "LEÃO" };

            var (grid, placements) = gen.Generate(8, 8, normalized, display);

            Assert.That(placements.Count, Is.GreaterThan(0));
            Assert.That(placements.Count, Is.LessThanOrEqualTo(3));
        }

        [Test]
        public void GridGenerator_Generate_WordsExistInGrid()
        {
            var gen = new GridGenerator(123);
            var normalized = new List<string> { "CASA", "MESA" };
            var display = new List<string> { "CASA", "MESA" };

            var (grid, placements) = gen.Generate(8, 8, normalized, display);

            foreach (var placement in placements)
            {
                var positions = placement.GetCellPositions();
                string word = "";
                foreach (var (row, col) in positions)
                {
                    word += grid.GetLetter(row, col);
                }
                Assert.That(word, Is.EqualTo(placement.NormalizedWord),
                    $"Word '{placement.NormalizedWord}' not found at expected positions.");
            }
        }

        [Test]
        public void GridGenerator_DeterministicSeed_SameResult()
        {
            var normalized = new List<string> { "GATO", "PATO", "LEAO", "SAPO" };
            var display = new List<string> { "GATO", "PATO", "LEÃO", "SAPO" };

            var gen1 = new GridGenerator(999);
            var (grid1, _) = gen1.Generate(8, 8, normalized, display);

            var gen2 = new GridGenerator(999);
            var (grid2, _) = gen2.Generate(8, 8, normalized, display);

            Assert.That(grid1.ToString(), Is.EqualTo(grid2.ToString()),
                "Same seed should produce identical grids.");
        }

        [Test]
        public void GridGenerator_DifferentSeed_DifferentResult()
        {
            var normalized = new List<string> { "GATO", "PATO", "LEAO", "SAPO" };
            var display = new List<string> { "GATO", "PATO", "LEÃO", "SAPO" };

            var gen1 = new GridGenerator(100);
            var (grid1, _) = gen1.Generate(8, 8, normalized, display);

            var gen2 = new GridGenerator(200);
            var (grid2, _) = gen2.Generate(8, 8, normalized, display);

            Assert.That(grid1.ToString(), Is.Not.EqualTo(grid2.ToString()),
                "Different seeds should produce different grids.");
        }

        [Test]
        public void GridGenerator_MismatchedWordLists_Throws()
        {
            var gen = new GridGenerator(42);
            var normalized = new List<string> { "GATO", "PATO" };
            var display = new List<string> { "GATO" };

            Assert.Throws<ArgumentException>(
                () => gen.Generate(8, 8, normalized, display));
        }

        // ═══════════════════════════════════════════════════
        //  WordFinder
        // ═══════════════════════════════════════════════════

        [Test]
        public void WordFinder_CheckSelection_FindsWord()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            var selection = new List<(int, int)> { (0, 0), (0, 1), (0, 2), (0, 3) };
            var found = finder.CheckSelection(selection);

            Assert.That(found, Is.Not.Null);
            Assert.That(found.NormalizedWord, Is.EqualTo("GATO"));
            Assert.That(found.Found, Is.True);
            Assert.That(finder.FoundCount, Is.EqualTo(1));
        }

        [Test]
        public void WordFinder_CheckSelection_ReverseDirection()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            // Seleção reversa: fim → início
            var selection = new List<(int, int)> { (0, 3), (0, 2), (0, 1), (0, 0) };
            var found = finder.CheckSelection(selection);

            Assert.That(found, Is.Not.Null);
            Assert.That(found.NormalizedWord, Is.EqualTo("GATO"));
        }

        [Test]
        public void WordFinder_CheckSelection_InvalidSelection_ReturnsNull()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            // Seleção errada
            var selection = new List<(int, int)> { (1, 0), (1, 1), (1, 2), (1, 3) };
            var found = finder.CheckSelection(selection);

            Assert.That(found, Is.Null);
        }

        [Test]
        public void WordFinder_CheckSelection_EmptySelection_ReturnsNull()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            Assert.That(finder.CheckSelection(null), Is.Null);
            Assert.That(finder.CheckSelection(new List<(int, int)>()), Is.Null);
        }

        [Test]
        public void WordFinder_CheckSelection_AlreadyFound_ReturnsNull()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            var selection = new List<(int, int)> { (0, 0), (0, 1), (0, 2), (0, 3) };
            finder.CheckSelection(selection); // Encontra primeira vez

            var found2 = finder.CheckSelection(selection); // Já encontrada
            Assert.That(found2, Is.Null);
        }

        [Test]
        public void WordFinder_AllWordsFound_EventFired()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("AB", "AB", 0, 0, Direction.Horizontal),
                new WordPlacement("CD", "CD", 1, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            bool allFoundFired = false;
            finder.OnAllWordsFound += () => allFoundFired = true;

            int wordFoundCount = 0;
            finder.OnWordFound += _ => wordFoundCount++;

            finder.CheckSelection(new List<(int, int)> { (0, 0), (0, 1) });
            Assert.That(allFoundFired, Is.False);
            Assert.That(wordFoundCount, Is.EqualTo(1));

            finder.CheckSelection(new List<(int, int)> { (1, 0), (1, 1) });
            Assert.That(allFoundFired, Is.True);
            Assert.That(wordFoundCount, Is.EqualTo(2));
            Assert.That(finder.AllFound, Is.True);
        }

        [Test]
        public void WordFinder_GetHint_ReturnsUnfoundWord()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal),
                new WordPlacement("PATO", "PATO", 1, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            // Encontrar primeira palavra
            finder.CheckSelection(new List<(int, int)> { (0, 0), (0, 1), (0, 2), (0, 3) });

            var hint = finder.GetHint(new Random(42));
            Assert.That(hint, Is.Not.Null);
            Assert.That(hint.NormalizedWord, Is.EqualTo("PATO"));
            Assert.That(hint.Found, Is.False);
        }

        [Test]
        public void WordFinder_GetHint_AllFound_ReturnsNull()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("AB", "AB", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            finder.CheckSelection(new List<(int, int)> { (0, 0), (0, 1) });

            var hint = finder.GetHint();
            Assert.That(hint, Is.Null);
        }

        [Test]
        public void WordFinder_RevealWord_MarksFound()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            bool eventFired = false;
            finder.OnWordFound += _ => eventFired = true;

            finder.RevealWord(placements[0]);

            Assert.That(placements[0].Found, Is.True);
            Assert.That(eventFired, Is.True);
            Assert.That(finder.FoundCount, Is.EqualTo(1));
        }

        [Test]
        public void WordFinder_RevealWord_AlreadyFound_NoEffect()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);
            placements[0].Found = true;

            int eventCount = 0;
            finder.OnWordFound += _ => eventCount++;

            finder.RevealWord(placements[0]);

            Assert.That(eventCount, Is.EqualTo(0));
        }

        [Test]
        public void WordFinder_TotalWords_Correct()
        {
            var placements = new List<WordPlacement>
            {
                new WordPlacement("A", "A", 0, 0, Direction.Horizontal),
                new WordPlacement("B", "B", 1, 0, Direction.Horizontal),
                new WordPlacement("C", "C", 2, 0, Direction.Horizontal)
            };
            var finder = new WordFinder(placements);

            Assert.That(finder.TotalWords, Is.EqualTo(3));
            Assert.That(finder.FoundCount, Is.EqualTo(0));
            Assert.That(finder.AllFound, Is.False);
        }

        [Test]
        public void WordFinder_Constructor_NullPlacements_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new WordFinder(null));
        }
    }
}
