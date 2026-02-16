using System;
using System.Collections.Generic;
using UnityEngine;
using RagazziStudios.Core.Domain.Level;
using RagazziStudios.Core.Domain.Words;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Storage;
using RagazziStudios.Core.Infrastructure.Analytics;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Gerencia progressão de níveis, save/load de progresso e geração de níveis.
    /// Coordena Domain (LevelGenerator, WordDatabase) com Infrastructure (Storage, Analytics).
    /// </summary>
    public class LevelManager
    {
        private const int LEVELS_PER_CATEGORY = 15;

        private readonly LevelGenerator _levelGenerator;
        private readonly WordDatabase _wordDatabase;

        /// <summary>Categoria selecionada atualmente.</summary>
        public string CurrentCategoryId { get; private set; }

        /// <summary>Número do nível atual.</summary>
        public int CurrentLevelNumber { get; private set; }

        /// <summary>Dados do nível sendo jogado.</summary>
        public LevelData CurrentLevel { get; private set; }

        /// <summary>Tempo de início do nível atual (para analytics).</summary>
        public float LevelStartTime { get; private set; }

        /// <summary>Dicas usadas no nível atual.</summary>
        public int HintsUsedInLevel { get; private set; }

        // --- Eventos ---

        /// <summary>Nível concluído. Args: (categoryId, levelNumber).</summary>
        public event Action<string, int> OnLevelComplete;

        /// <summary>Categoria selecionada.</summary>
        public event Action<string> OnCategorySelected;

        public LevelManager()
        {
            _levelGenerator = new LevelGenerator();
            _wordDatabase = new WordDatabase();
        }

        /// <summary>
        /// Referência ao WordDatabase para carregar palavras externamente.
        /// </summary>
        public WordDatabase WordDatabase => _wordDatabase;

        // --- Seleção ---

        /// <summary>
        /// Seleciona uma categoria.
        /// </summary>
        public void SelectCategory(string categoryId)
        {
            CurrentCategoryId = categoryId;
            OnCategorySelected?.Invoke(categoryId);

            if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            {
                analytics.TrackCategorySelected(categoryId);
            }
        }

        /// <summary>
        /// Gera e retorna um nível para jogar.
        /// </summary>
        public LevelData StartLevel(int levelNumber)
        {
            if (string.IsNullOrEmpty(CurrentCategoryId))
            {
                Debug.LogError("[LevelManager] No category selected!");
                return null;
            }

            if (!_wordDatabase.HasCategory(CurrentCategoryId))
            {
                Debug.LogError($"[LevelManager] Category '{CurrentCategoryId}' not loaded in WordDatabase!");
                return null;
            }

            CurrentLevelNumber = levelNumber;
            HintsUsedInLevel = 0;
            IsChallengeMode = false;
            LevelStartTime = Time.realtimeSinceStartup;

            var category = _wordDatabase.GetCategory(CurrentCategoryId);

            CurrentLevel = _levelGenerator.Generate(
                CurrentCategoryId, levelNumber,
                category.NormalizedWords, category.DisplayWords);

            // Analytics
            if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            {
                string difficulty = levelNumber <= 5 ? "easy" :
                    levelNumber <= 10 ? "medium" : "hard";
                analytics.TrackLevelStart(CurrentCategoryId, levelNumber, difficulty);
            }

            Debug.Log($"[LevelManager] Level started: {CurrentCategoryId} #{levelNumber} " +
                      $"(grid {CurrentLevel.Difficulty.GridRows}x{CurrentLevel.Difficulty.GridCols}, " +
                      $"{CurrentLevel.Placements.Count} words)");

            return CurrentLevel;
        }

        /// <summary>
        /// Indica se o modo atual é Desafio.
        /// </summary>
        public bool IsChallengeMode { get; private set; }

        /// <summary>
        /// Gera um nível de desafio com grid grande e palavras mistas de todas as categorias.
        /// </summary>
        /// <param name="gridRows">Linhas do grid (ex: 14, 18, 20).</param>
        /// <param name="gridCols">Colunas do grid (ex: 22).</param>
        /// <param name="wordCount">Quantidade de palavras (10).</param>
        public LevelData StartChallengeLevel(int gridRows, int gridCols, int wordCount = 10)
        {
            IsChallengeMode = true;
            CurrentCategoryId = "desafio";
            CurrentLevelNumber = 1;
            HintsUsedInLevel = 0;
            LevelStartTime = Time.realtimeSinceStartup;

            // Coletar palavras de todas as categorias carregadas
            var allNormalized = new List<string>();
            var allDisplay = new List<string>();

            foreach (var catId in _wordDatabase.GetCategoryIds())
            {
                var cat = _wordDatabase.GetCategory(catId);
                allNormalized.AddRange(cat.NormalizedWords);
                allDisplay.AddRange(cat.DisplayWords);
            }

            if (allNormalized.Count == 0)
            {
                Debug.LogError("[LevelManager] No words available for challenge!");
                return null;
            }

            // Criar DifficultyConfig customizado
            var difficulty = new DifficultyConfig(gridRows, gridCols, wordCount, wordCount);

            // Seed baseado no tempo para variar cada partida
            int seed = System.Environment.TickCount;
            var random = new System.Random(seed);

            // Selecionar palavras aleatórias que caibam no grid
            var validIndices = new List<int>();
            int maxLen = Mathf.Max(gridRows, gridCols);
            for (int i = 0; i < allNormalized.Count; i++)
            {
                if (allNormalized[i].Length >= 3 && allNormalized[i].Length <= maxLen)
                    validIndices.Add(i);
            }

            // Shuffle
            for (int i = validIndices.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int tmp = validIndices[i];
                validIndices[i] = validIndices[j];
                validIndices[j] = tmp;
            }

            int count = Mathf.Min(wordCount, validIndices.Count);
            var selectedNorm = new List<string>(count);
            var selectedDisp = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int idx = validIndices[i];
                selectedNorm.Add(allNormalized[idx]);
                selectedDisp.Add(allDisplay[idx]);
            }

            // Gerar grid
            var gridGen = new Domain.Grid.GridGenerator(seed);
            var (grid, placements) = gridGen.Generate(gridRows, gridCols,
                selectedNorm, selectedDisp);

            CurrentLevel = new LevelData("desafio", 1, seed, difficulty, grid, placements);

            Debug.Log($"[LevelManager] Challenge started: {gridRows}x{gridCols}, " +
                      $"{CurrentLevel.Placements.Count} words from all categories");

            return CurrentLevel;
        }

        /// <summary>
        /// Registra o uso de uma dica no nível atual.
        /// </summary>
        public void RegisterHint()
        {
            HintsUsedInLevel++;
        }

        /// <summary>
        /// Marca o nível atual como completo. Salva progresso e dispara eventos.
        /// </summary>
        public void CompleteLevel()
        {
            float timeSeconds = Time.realtimeSinceStartup - LevelStartTime;

            // Salvar progresso
            if (ServiceLocator.TryGet<IStorageService>(out var storage))
            {
                // Marcar nível como completo
                string key = StorageKeys.LevelCompleted(CurrentCategoryId, CurrentLevelNumber);
                storage.SetInt(key, 1);

                // Atualizar maior nível desbloqueado
                string unlockedKey = StorageKeys.HighestUnlocked(CurrentCategoryId);
                int currentHighest = storage.GetInt(unlockedKey, 1);
                int nextLevel = CurrentLevelNumber + 1;

                if (nextLevel > currentHighest && nextLevel <= LEVELS_PER_CATEGORY)
                {
                    storage.SetInt(unlockedKey, nextLevel);
                }

                storage.Save();
            }

            // Analytics
            if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            {
                analytics.TrackLevelComplete(
                    CurrentCategoryId, CurrentLevelNumber,
                    timeSeconds, HintsUsedInLevel);
            }

            OnLevelComplete?.Invoke(CurrentCategoryId, CurrentLevelNumber);

            Debug.Log($"[LevelManager] Level complete: {CurrentCategoryId} #{CurrentLevelNumber} " +
                      $"({timeSeconds:F1}s, {HintsUsedInLevel} hints)");
        }

        /// <summary>
        /// Registra que o jogador saiu do nível sem completar.
        /// </summary>
        public void QuitLevel(int wordsFound)
        {
            float timeSeconds = Time.realtimeSinceStartup - LevelStartTime;

            if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            {
                analytics.TrackLevelQuit(
                    CurrentCategoryId, CurrentLevelNumber,
                    timeSeconds, wordsFound);
            }

            Debug.Log($"[LevelManager] Level quit: {CurrentCategoryId} #{CurrentLevelNumber} " +
                      $"({timeSeconds:F1}s, {wordsFound} words found)");
        }

        // --- Progresso ---

        /// <summary>
        /// Verifica se um nível está completo.
        /// </summary>
        public bool IsLevelCompleted(string categoryId, int levelNumber)
        {
            if (!ServiceLocator.TryGet<IStorageService>(out var storage))
                return false;

            string key = StorageKeys.LevelCompleted(categoryId, levelNumber);
            return storage.GetInt(key, 0) == 1;
        }

        /// <summary>
        /// Verifica se um nível está desbloqueado.
        /// Nível 1 sempre desbloqueado. Demais requerem completar o anterior.
        /// </summary>
        public bool IsLevelUnlocked(string categoryId, int levelNumber)
        {
            if (levelNumber <= 1)
                return true;

            if (!ServiceLocator.TryGet<IStorageService>(out var storage))
                return levelNumber == 1;

            string unlockedKey = StorageKeys.HighestUnlocked(categoryId);
            int highest = storage.GetInt(unlockedKey, 1);
            return levelNumber <= highest;
        }

        /// <summary>
        /// Retorna o progresso de uma categoria (quantidade de níveis completos).
        /// </summary>
        public int GetCategoryProgress(string categoryId)
        {
            int completed = 0;
            for (int i = 1; i <= LEVELS_PER_CATEGORY; i++)
            {
                if (IsLevelCompleted(categoryId, i))
                    completed++;
            }
            return completed;
        }

        /// <summary>
        /// Total de níveis por categoria.
        /// </summary>
        public int LevelsPerCategory => LEVELS_PER_CATEGORY;

        /// <summary>
        /// Verifica se há próximo nível disponível.
        /// </summary>
        public bool HasNextLevel =>
            CurrentLevelNumber < LEVELS_PER_CATEGORY;

        /// <summary>
        /// Número do próximo nível (ou -1 se não há).
        /// </summary>
        public int NextLevelNumber =>
            HasNextLevel ? CurrentLevelNumber + 1 : -1;
    }
}
