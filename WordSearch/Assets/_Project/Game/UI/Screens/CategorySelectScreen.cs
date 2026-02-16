using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Domain.Words;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Tela de seleção de categorias.
    /// Exibe um grid de botões com ícone, nome e progresso de cada categoria.
    /// </summary>
    public class CategorySelectScreen : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Transform _categoryContainer;
        [SerializeField] private GameObject _categoryButtonPrefab;

        [Header("UI")]
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private Button _backButton;

        private const string CATEGORIES_PATH = "Data/categories";

        private void OnEnable()
        {
            _backButton.onClick.AddListener(OnBackClicked);
            PopulateCategories();
            UpdateLocalization();
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void UpdateLocalization()
        {
            if (!ServiceLocator.TryGet<ILocalizationService>(out var loc))
                return;

            if (_headerText != null)
                _headerText.text = loc.Get("title_categories");
        }

        private void PopulateCategories()
        {
            // Limpar filhos existentes
            foreach (Transform child in _categoryContainer)
            {
                Destroy(child.gameObject);
            }

            // Carregar categorias
            var categoriesAsset = Resources.Load<TextAsset>(CATEGORIES_PATH);
            if (categoriesAsset == null)
            {
                Debug.LogError("[CategorySelectScreen] categories.json not found!");
                return;
            }

            var categoriesFile = JsonUtility.FromJson<CategoriesFile>(categoriesAsset.text);
            if (categoriesFile?.categories == null) return;

            var levelManager = GameManager.Instance?.LevelManager;

            foreach (var category in categoriesFile.categories)
            {
                var buttonObj = Instantiate(_categoryButtonPrefab, _categoryContainer);
                var item = buttonObj.GetComponent<CategoryButtonItem>();

                if (item != null && levelManager != null)
                {
                    int progress = levelManager.GetCategoryProgress(category.id);
                    int total = levelManager.LevelsPerCategory;
                    item.Setup(category, progress, total, OnCategoryClicked);
                }
            }
        }

        private void OnCategoryClicked(string categoryId)
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.LevelManager.SelectCategory(categoryId);
            GameManager.Instance.StateMachine.TransitionTo(GameStateType.LevelSelect);
        }

        private void OnBackClicked()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.StateMachine.GoBack();
        }
    }
}
