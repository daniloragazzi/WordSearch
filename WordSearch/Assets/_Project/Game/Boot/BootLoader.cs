using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Domain.Words;

namespace RagazziStudios.Game.Boot
{
    /// <summary>
    /// Script da cena Boot. Carrega todos os dados (categorias + palavras)
    /// no WordDatabase e transiciona para MainMenu quando pronto.
    /// Deve ser o único script na cena Boot junto com GameManager.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        private const string CATEGORIES_PATH = "Data/categories";
        private const string WORDS_PATH_PREFIX = "Data/words/";
        private const string MAIN_MENU_SCENE = "MainMenu";

        [Header("UI")]
        [SerializeField] private CanvasGroup _loadingCanvasGroup;

        private void Start()
        {
            StartCoroutine(LoadGameData());
        }

        private IEnumerator LoadGameData()
        {
            Debug.Log("[BootLoader] Loading game data...");

            // Aguardar GameManager estar pronto
            while (GameManager.Instance == null)
            {
                yield return null;
            }

            var levelManager = GameManager.Instance.LevelManager;
            var wordDatabase = levelManager.WordDatabase;

            // Carregar categories.json
            var categoriesAsset = Resources.Load<TextAsset>(CATEGORIES_PATH);
            if (categoriesAsset == null)
            {
                Debug.LogError("[BootLoader] categories.json not found!");
                yield break;
            }

            var categoriesFile = JsonUtility.FromJson<CategoriesFile>(categoriesAsset.text);
            if (categoriesFile?.categories == null)
            {
                Debug.LogError("[BootLoader] Failed to parse categories.json!");
                yield break;
            }

            Debug.Log($"[BootLoader] Found {categoriesFile.categories.Count} categories.");

            // Carregar palavras de cada categoria
            int loaded = 0;
            foreach (var category in categoriesFile.categories)
            {
                var wordsAsset = Resources.Load<TextAsset>(WORDS_PATH_PREFIX + category.id);
                if (wordsAsset == null)
                {
                    Debug.LogWarning($"[BootLoader] Words file not found for '{category.id}'.");
                    continue;
                }

                var wordsFile = JsonUtility.FromJson<WordsFile>(wordsAsset.text);
                if (wordsFile?.words == null)
                {
                    Debug.LogWarning($"[BootLoader] Failed to parse words for '{category.id}'.");
                    continue;
                }

                wordDatabase.AddCategory(category.id, wordsFile.words);
                loaded++;

                Debug.Log($"[BootLoader] Loaded '{category.id}': {wordsFile.words.Count} words.");

                // Yield a cada categoria para não travar frame
                yield return null;
            }

            Debug.Log($"[BootLoader] Data loaded: {loaded}/{categoriesFile.categories.Count} categories.");

            // Pequeno delay para a splash ser visível
            yield return new WaitForSeconds(0.5f);

            // Fade out (se houver canvas)
            if (_loadingCanvasGroup != null)
            {
                float elapsed = 0f;
                float fadeDuration = 0.3f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    _loadingCanvasGroup.alpha = 1f - (elapsed / fadeDuration);
                    yield return null;
                }
            }

            // Transicionar para MainMenu com fade
            var gm = Core.Application.GameManager.Instance;
            if (gm != null)
            {
                gm.LoadScene(MAIN_MENU_SCENE);
            }
            else
            {
                SceneManager.LoadScene(MAIN_MENU_SCENE);
            }
        }
    }
}
