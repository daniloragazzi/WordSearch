using System.Collections.Generic;
using System.Linq;
using System.Linq;
using UnityEngine;
using RagazziStudios.Core.Domain.Grid;
using RagazziStudios.Core.Domain.Level;
using RagazziStudios.Core.Domain.Words;

namespace RagazziStudios.Tests
{
    /// <summary>
    /// Script de teste de integração end-to-end executável no Editor.
    /// Valida todo o pipeline: WordDatabase → LevelGenerator → GridGenerator → WordFinder.
    /// 
    /// USO:
    /// 1. Criar GameObject vazio na scene
    /// 2. Adicionar este componente
    /// 3. Play → verificar Console para resultados
    /// 
    /// Ou via menu: [CONTEXTE_MENU] Run Integration Test
    /// </summary>
    public class IntegrationTestRunner : MonoBehaviour
    {
        [Header("Configuração")]
        [SerializeField] private bool _runOnStart = true;
        [SerializeField] private bool _verboseOutput = true;

        private int _passCount;
        private int _failCount;

        private void Start()
        {
            if (_runOnStart)
                RunAllTests();
        }

        [ContextMenu("Run Integration Test")]
        public void RunAllTests()
        {
            _passCount = 0;
            _failCount = 0;

            Debug.Log("╔══════════════════════════════════════════════╗");
            Debug.Log("║     INTEGRATION TEST — Caça-Palavras        ║");
            Debug.Log("╚══════════════════════════════════════════════╝");

            Test_WordDatabase_LoadAndNormalize();
            Test_LevelGenerator_AllLevelsAllCategories();
            Test_WordFinder_CompleteFlow();
            Test_DeterministicReplay();
            Test_GridFill_NoEmptyCells();

            Debug.Log("══════════════════════════════════════════════");
            Debug.Log($"RESULTADO: {_passCount} passed, {_failCount} failed, " +
                      $"{_passCount + _failCount} total");

            if (_failCount == 0)
                Debug.Log("<color=green>✅ TODOS OS TESTES PASSARAM!</color>");
            else
                Debug.LogError($"❌ {_failCount} TESTE(S) FALHARAM!");
        }

        // ═══════════════════════════════════════════════════
        //  Test 1: WordDatabase — Load + Normalize
        // ═══════════════════════════════════════════════════

        private void Test_WordDatabase_LoadAndNormalize()
        {
            string testName = "WordDatabase.LoadAndNormalize";

            try
            {
                var db = new WordDatabase();

                var animaisWords = new List<string>
                {
                    "gato", "cachorro", "elefante", "leão", "pato",
                    "cavalo", "cobra", "sapo", "urso", "tigre",
                    "girafa", "macaco", "lobo", "raposa", "foca",
                    "baleia", "golfinho", "tartaruga", "papagaio", "borboleta"
                };

                db.AddCategory("animais", animaisWords);

                Assert(testName, db.HasCategory("animais"), "HasCategory should be true");
                Assert(testName, db.CategoryCount == 1, $"CategoryCount should be 1, got {db.CategoryCount}");

                var cat = db.GetCategory("animais");
                Assert(testName, cat.WordCount == 20, $"WordCount should be 20, got {cat.WordCount}");

                // Verificar normalização
                Assert(testName, cat.NormalizedWords.Contains("LEAO"),
                    "Normalized 'leão' should be 'LEAO'");
                Assert(testName, cat.DisplayWords.Contains("LEÃO"),
                    "Display 'leão' should be 'LEÃO'");

                Pass(testName);
            }
            catch (System.Exception e)
            {
                Fail(testName, e.Message);
            }
        }

        // ═══════════════════════════════════════════════════
        //  Test 2: LevelGenerator — All 15 Levels × 8 Categories
        // ═══════════════════════════════════════════════════

        private void Test_LevelGenerator_AllLevelsAllCategories()
        {
            string testName = "LevelGenerator.AllLevelsAllCategories";

            try
            {
                var categories = new string[]
                {
                    "animais", "frutas", "cores", "profissoes",
                    "esportes", "paises", "corpo_humano", "natureza"
                };

                // Criar banco com 20 palavras genéricas por categoria
                var db = new WordDatabase();
                foreach (var catId in categories)
                {
                    var words = GenerateTestWords(catId, 55);
                    db.AddCategory(catId, words);
                }

                var gen = new LevelGenerator();
                int levelsGenerated = 0;

                foreach (var catId in categories)
                {
                    var cat = db.GetCategory(catId);

                    for (int lvl = 1; lvl <= 15; lvl++)
                    {
                        var level = gen.Generate(catId, lvl,
                            cat.NormalizedWords, cat.DisplayWords);

                        Assert(testName, level.Grid != null,
                            $"[{catId}/L{lvl}] Grid should not be null");
                        Assert(testName, level.Grid.IsFilled(),
                            $"[{catId}/L{lvl}] Grid should be filled");
                        Assert(testName, level.Placements.Count >= 1,
                            $"[{catId}/L{lvl}] Should have at least 1 placement");

                        var difficulty = DifficultyConfig.ForLevel(lvl);
                        Assert(testName, level.Grid.Rows == difficulty.GridRows,
                            $"[{catId}/L{lvl}] Grid rows {level.Grid.Rows} != {difficulty.GridRows}");

                        levelsGenerated++;
                    }
                }

                Assert(testName, levelsGenerated == 120,
                    $"Should generate 120 levels, got {levelsGenerated}");

                Pass(testName);

                if (_verboseOutput)
                    Debug.Log($"  → {levelsGenerated} níveis gerados com sucesso (8 categorias × 15 níveis)");
            }
            catch (System.Exception e)
            {
                Fail(testName, e.Message);
            }
        }

        // ═══════════════════════════════════════════════════
        //  Test 3: WordFinder — Complete Flow
        // ═══════════════════════════════════════════════════

        private void Test_WordFinder_CompleteFlow()
        {
            string testName = "WordFinder.CompleteFlow";

            try
            {
                // Criar placements conhecidos
                var placements = new List<WordPlacement>
                {
                    new WordPlacement("GATO", "GATO", 0, 0, Direction.Horizontal),
                    new WordPlacement("PATO", "PATO", 1, 0, Direction.Horizontal),
                    new WordPlacement("SAPO", "SAPO", 2, 0, Direction.Horizontal)
                };

                var finder = new WordFinder(placements);

                Assert(testName, finder.TotalWords == 3, "Total words should be 3");
                Assert(testName, finder.FoundCount == 0, "Found count should start at 0");
                Assert(testName, !finder.AllFound, "AllFound should be false initially");

                // Encontrar GATO
                int wordFoundEvents = 0;
                bool allFoundFired = false;
                finder.OnWordFound += _ => wordFoundEvents++;
                finder.OnAllWordsFound += () => allFoundFired = true;

                var result1 = finder.CheckSelection(
                    new List<(int, int)> { (0, 0), (0, 1), (0, 2), (0, 3) });
                Assert(testName, result1 != null && result1.NormalizedWord == "GATO",
                    "Should find GATO");

                // Encontrar PATO (reverso)
                var result2 = finder.CheckSelection(
                    new List<(int, int)> { (1, 3), (1, 2), (1, 1), (1, 0) });
                Assert(testName, result2 != null && result2.NormalizedWord == "PATO",
                    "Should find PATO in reverse");

                Assert(testName, !allFoundFired, "AllFound should not fire yet");

                // Encontrar SAPO
                var result3 = finder.CheckSelection(
                    new List<(int, int)> { (2, 0), (2, 1), (2, 2), (2, 3) });
                Assert(testName, result3 != null, "Should find SAPO");

                Assert(testName, allFoundFired, "AllFound should fire after all words found");
                Assert(testName, finder.AllFound, "AllFound property should be true");
                Assert(testName, wordFoundEvents == 3, "Should have fired 3 OnWordFound events");

                // Hint quando tudo já foi encontrado
                var hint = finder.GetHint();
                Assert(testName, hint == null, "Hint should be null when all found");

                Pass(testName);
            }
            catch (System.Exception e)
            {
                Fail(testName, e.Message);
            }
        }

        // ═══════════════════════════════════════════════════
        //  Test 4: Deterministic Replay
        // ═══════════════════════════════════════════════════

        private void Test_DeterministicReplay()
        {
            string testName = "LevelGenerator.DeterministicReplay";

            try
            {
                var words = GenerateTestWords("test", 30);
                var db = new WordDatabase();
                db.AddCategory("test", words);
                var cat = db.GetCategory("test");

                var gen = new LevelGenerator();

                // Gerar mesmo nível 3 vezes
                var level1 = gen.Generate("test", 7, cat.NormalizedWords, cat.DisplayWords);
                var level2 = gen.Generate("test", 7, cat.NormalizedWords, cat.DisplayWords);
                var level3 = gen.Generate("test", 7, cat.NormalizedWords, cat.DisplayWords);

                string grid1 = level1.Grid.ToString();
                string grid2 = level2.Grid.ToString();
                string grid3 = level3.Grid.ToString();

                Assert(testName, grid1 == grid2, "Run 1 vs 2 should be identical");
                Assert(testName, grid2 == grid3, "Run 2 vs 3 should be identical");
                Assert(testName, level1.Placements.Count == level2.Placements.Count,
                    "Placement count should match");

                // Verificar que palavras colocadas são as mesmas
                for (int i = 0; i < level1.Placements.Count; i++)
                {
                    Assert(testName,
                        level1.Placements[i].NormalizedWord == level2.Placements[i].NormalizedWord,
                        $"Placement [{i}] word should match");
                }

                Pass(testName);

                if (_verboseOutput)
                    Debug.Log("  → 3 execuções idênticas confirmadas (seed determinístico OK)");
            }
            catch (System.Exception e)
            {
                Fail(testName, e.Message);
            }
        }

        // ═══════════════════════════════════════════════════
        //  Test 5: Grid Fill — No Empty Cells
        // ═══════════════════════════════════════════════════

        private void Test_GridFill_NoEmptyCells()
        {
            string testName = "GridGenerator.NoEmptyCells";

            try
            {
                var sizes = new int[] { 8, 10, 12 };
                int gridsChecked = 0;

                foreach (int size in sizes)
                {
                    for (int seed = 0; seed < 10; seed++)
                    {
                        var gen = new GridGenerator(seed);
                        var words = new List<string> { "TESTE", "GATO", "PATO" };
                        var display = new List<string> { "TESTE", "GATO", "PATO" };

                        var (grid, _) = gen.Generate(size, size, words, display);

                        Assert(testName, grid.IsFilled(),
                            $"Grid {size}x{size} seed={seed} should be filled");
                        Assert(testName, grid.CountEmpty() == 0,
                            $"Grid {size}x{size} seed={seed} should have 0 empty cells");

                        gridsChecked++;
                    }
                }

                Pass(testName);

                if (_verboseOutput)
                    Debug.Log($"  → {gridsChecked} grids verificadas (3 tamanhos × 10 seeds)");
            }
            catch (System.Exception e)
            {
                Fail(testName, e.Message);
            }
        }

        // ═══════════════════════════════════════════════════
        //  Helpers
        // ═══════════════════════════════════════════════════

        private List<string> GenerateTestWords(string prefix, int count)
        {
            var words = new List<string>();
            var baseWords = new string[]
            {
                "gato", "pato", "sapo", "lobo", "urso",
                "cobra", "foca", "leão", "tigre", "cavalo",
                "girafa", "macaco", "baleia", "raposa", "coruja",
                "golfinho", "tartaruga", "papagaio", "elefante", "borboleta",
                "cachorro", "coelho", "galinha", "ovelha", "porco",
                "jacaré", "pinguim", "flamingo", "tubarão", "águia",
                "veado", "pantera", "búfalo", "camelo", "hiena",
                "morsa", "lontra", "arara", "tucano", "pavão",
                "lhama", "capivara", "quati", "tatu", "preguiça",
                "onça", "piranha", "sardinha", "lagosta", "camarão",
                "polvo", "medusa", "estrela", "cavalo", "touro"
            };

            for (int i = 0; i < count && i < baseWords.Length; i++)
            {
                words.Add(baseWords[i]);
            }

            return words;
        }

        private void Assert(string testName, bool condition, string message)
        {
            if (!condition)
            {
                throw new System.Exception($"Assertion failed: {message}");
            }
        }

        private void Pass(string testName)
        {
            _passCount++;
            Debug.Log($"<color=green>✅ PASS</color> — {testName}");
        }

        private void Fail(string testName, string error)
        {
            _failCount++;
            Debug.LogError($"❌ FAIL — {testName}: {error}");
        }
    }
}
