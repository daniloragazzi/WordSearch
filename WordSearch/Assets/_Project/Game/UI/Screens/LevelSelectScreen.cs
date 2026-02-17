using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;
using RagazziStudios.Core.Domain;

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

        [Header("Tema")]
        [SerializeField] private GameTheme _theme;

        [Header("Cores dos Estados")]
        [SerializeField] private Color _completedColor = new Color(0.3f, 0.8f, 0.3f);
        [SerializeField] private Color _unlockedColor = new Color(0.20f, 0.47f, 0.96f);
        [SerializeField] private Color _lockedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        private Color _completedTextColor = Color.white;
        private Color _unlockedTextColor = Color.white;
        private Color _lockedTextColor = Color.white;

        private void OnEnable()
        {
            if (_backButton != null)
                _backButton.onClick.AddListener(OnBackClicked);

            ThemeManager.OnThemeChanged += OnThemeChanged;
            ResolveTheme();
            ApplyThemeColors();
            PopulateLevels();
            UpdateLocalization();
        }

        private void OnDisable()
        {
            if (_backButton != null)
                _backButton.onClick.RemoveListener(OnBackClicked);

            ThemeManager.OnThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(GameTheme newTheme)
        {
            _theme = newTheme;
            ApplyThemeColors();
            PopulateLevels();
        }

        private void ResolveTheme()
        {
            if (_theme == null && ThemeManager.Instance != null)
                _theme = ThemeManager.Instance.CurrentTheme;
        }

        private void ApplyThemeColors()
        {
            if (_theme == null) return;

            _completedColor = _theme.success;
            _unlockedColor = _theme.primary;
            _lockedColor = new Color(
                _theme.textDisabled.r,
                _theme.textDisabled.g,
                _theme.textDisabled.b,
                0.5f);

            _completedTextColor = _theme.textOnColor;
            _unlockedTextColor = _theme.textOnColor;
            _lockedTextColor = _theme.textDisabled;

            if (_headerText != null)
                _headerText.color = _theme.textPrimary;

            if (_categoryNameText != null)
                _categoryNameText.color = _theme.textSecondary;
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
                    Color textColor = completed ? _completedTextColor :
                        unlocked ? _unlockedTextColor : _lockedTextColor;

                    item.Setup(i, completed, unlocked, color, textColor, OnLevelClicked);
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
