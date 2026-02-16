using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Tela de seleção de níveis (15 por categoria).
    /// Mostra botões numerados com estados: completo, desbloqueado, bloqueado.
    /// </summary>
    public class LevelSelectScreen : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Transform _levelContainer;
        [SerializeField] private GameObject _levelButtonPrefab;

        [Header("UI")]
        [SerializeField] private TMP_Text _headerText;
        [SerializeField] private TMP_Text _categoryNameText;
        [SerializeField] private Button _backButton;

        [Header("Cores dos Estados")]
        [SerializeField] private Color _completedColor = new Color(0.3f, 0.8f, 0.3f);
        [SerializeField] private Color _unlockedColor = new Color(1f, 1f, 1f);
        [SerializeField] private Color _lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private void OnEnable()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);
            PopulateLevels();
            UpdateLocalization();
        }

        private void OnDisable()
        {
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void UpdateLocalization()
        {
            if (!ServiceLocator.TryGet<ILocalizationService>(out var loc))
                return;

            if (_headerText != null)
                _headerText.text = loc.Get("title_levels");
        }

        private void PopulateLevels()
        {
            // Limpar filhos
            foreach (Transform child in _levelContainer)
            {
                Destroy(child.gameObject);
            }

            var levelManager = GameManager.Instance?.LevelManager;
            if (levelManager == null) return;

            string categoryId = levelManager.CurrentCategoryId;

            // Atualizar nome da categoria no header
            if (_categoryNameText != null)
            {
                // Carregar nome da categoria do JSON
                var categoriesAsset = Resources.Load<TextAsset>("Data/categories");
                if (categoriesAsset != null)
                {
                    var file = JsonUtility.FromJson<Core.Domain.Words.CategoriesFile>(
                        categoriesAsset.text);
                    var cat = file?.categories?.Find(c => c.id == categoryId);
                    if (cat != null) _categoryNameText.text = cat.name;
                }
            }

            // Criar botões de nível
            for (int i = 1; i <= levelManager.LevelsPerCategory; i++)
            {
                var buttonObj = Instantiate(_levelButtonPrefab, _levelContainer);
                buttonObj.SetActive(true);
                var item = buttonObj.GetComponent<LevelButtonItem>();

                if (item != null)
                {
                    bool completed = levelManager.IsLevelCompleted(categoryId, i);
                    bool unlocked = levelManager.IsLevelUnlocked(categoryId, i);

                    Color color = completed ? _completedColor :
                        unlocked ? _unlockedColor : _lockedColor;

                    item.Setup(i, completed, unlocked, color, OnLevelClicked);
                }
            }
        }

        private void OnLevelClicked(int levelNumber)
        {
            if (GameManager.Instance == null) return;

            var levelManager = GameManager.Instance.LevelManager;
            string categoryId = levelManager.CurrentCategoryId;

            // Só permitir se desbloqueado
            if (!levelManager.IsLevelUnlocked(categoryId, levelNumber))
                return;

            // Gerar e iniciar nível
            levelManager.StartLevel(levelNumber);

            // Transicionar para gameplay
            GameManager.Instance.StateMachine.TransitionTo(GameStateType.Playing);
        }

        private void OnBackClicked()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.StateMachine.GoBack();
        }
    }
}
