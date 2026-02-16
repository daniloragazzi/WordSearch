using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;

namespace RagazziStudios.Game.UI.Popups
{
    /// <summary>
    /// Popup de vitória exibido ao completar um nível.
    /// Mostra parabéns, estatísticas e botões de ação.
    /// </summary>
    public class WinPopup : MonoBehaviour
    {
        [Header("Textos")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private TMP_Text _statsText;

        [Header("Botões")]
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private TMP_Text _nextLevelButtonText;
        [SerializeField] private Button _levelSelectButton;
        [SerializeField] private TMP_Text _levelSelectButtonText;

        [Header("Animação")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _popupPanel;
        [SerializeField] private float _animationDuration = 0.3f;

        private string _categoryId;
        private int _levelNumber;
        private bool _hasNextLevel;

        /// <summary>
        /// Configura o popup com os dados do nível completado.
        /// </summary>
        public void Setup(string categoryId, int levelNumber,
            int wordsFound, float timeSeconds, bool hasNextLevel)
        {
            _categoryId = categoryId;
            _levelNumber = levelNumber;
            _hasNextLevel = hasNextLevel;

            UpdateLocalization();

            // Estatísticas
            if (_statsText != null)
            {
                int minutes = Mathf.FloorToInt(timeSeconds / 60f);
                int seconds = Mathf.FloorToInt(timeSeconds % 60f);

                if (ServiceLocator.TryGet<ILocalizationService>(out var loc))
                {
                    _statsText.text = loc.Get("win_stats", wordsFound, minutes, seconds);
                }
                else
                {
                    _statsText.text = $"{wordsFound} palavras • {minutes}:{seconds:D2}";
                }
            }

            // Botão próximo nível
            if (_nextLevelButton != null)
            {
                _nextLevelButton.gameObject.SetActive(hasNextLevel);
                _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            }

            // Botão voltar para níveis
            if (_levelSelectButton != null)
            {
                _levelSelectButton.onClick.AddListener(OnLevelSelectClicked);
            }

            // Animação de entrada
            PlayEnterAnimation();
        }

        private void UpdateLocalization()
        {
            if (!ServiceLocator.TryGet<ILocalizationService>(out var loc))
                return;

            if (_titleText != null)
                _titleText.text = loc.Get("win_title");

            if (_messageText != null)
                _messageText.text = loc.Get("win_message");

            if (_nextLevelButtonText != null)
                _nextLevelButtonText.text = loc.Get("btn_next_level");

            if (_levelSelectButtonText != null)
                _levelSelectButtonText.text = loc.Get("btn_menu");
        }

        private void PlayEnterAnimation()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                StartCoroutine(AnimateIn());
            }

            if (_popupPanel != null)
            {
                _popupPanel.localScale = Vector3.one * 0.8f;
            }
        }

        private System.Collections.IEnumerator AnimateIn()
        {
            float elapsed = 0f;
            Vector3 startScale = Vector3.one * 0.8f;
            Vector3 endScale = Vector3.one;

            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _animationDuration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = t;

                if (_popupPanel != null)
                    _popupPanel.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return null;
            }

            if (_canvasGroup != null) _canvasGroup.alpha = 1f;
            if (_popupPanel != null) _popupPanel.localScale = Vector3.one;
        }

        private void OnNextLevelClicked()
        {
            if (GameManager.Instance == null) return;

            var levelManager = GameManager.Instance.LevelManager;

            if (levelManager.IsChallengeMode)
            {
                // Gerar novo desafio com mesmo tamanho de grid
                var diff = levelManager.CurrentLevel?.Difficulty;
                if (diff != null)
                    levelManager.StartChallengeLevel(diff.GridRows, diff.GridCols, 10);

                GameManager.Instance.StateMachine.TransitionTo(GameStateType.Playing);
                GameManager.Instance.LoadScene("Game");
            }
            else
            {
                int nextLevel = levelManager.NextLevelNumber;

                if (nextLevel > 0)
                {
                    levelManager.StartLevel(nextLevel);
                    GameManager.Instance.StateMachine.TransitionTo(GameStateType.Playing);
                    GameManager.Instance.LoadScene("Game");
                }
            }

            Destroy(gameObject);
        }

        private void OnLevelSelectClicked()
        {
            if (GameManager.Instance == null) return;

            var levelManager = GameManager.Instance.LevelManager;
            var targetState = levelManager.IsChallengeMode
                ? GameStateType.ChallengeSelect
                : GameStateType.LevelSelect;

            GameManager.Instance.StateMachine.TransitionTo(targetState);
            GameManager.Instance.LoadScene("MainMenu");

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_nextLevelButton != null)
                _nextLevelButton.onClick.RemoveListener(OnNextLevelClicked);

            if (_levelSelectButton != null)
                _levelSelectButton.onClick.RemoveListener(OnLevelSelectClicked);
        }
    }
}
