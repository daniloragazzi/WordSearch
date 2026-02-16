using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;

namespace RagazziStudios.Game.UI.Popups
{
    /// <summary>
    /// Popup de pausa exibido durante o gameplay.
    /// Botões: Continuar, Reiniciar, Menu (sair).
    /// </summary>
    public class PausePopup : MonoBehaviour
    {
        [Header("Textos")]
        [SerializeField] private TMP_Text _titleText;

        [Header("Botões")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private TMP_Text _continueButtonText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text _restartButtonText;
        [SerializeField] private Button _menuButton;
        [SerializeField] private TMP_Text _menuButtonText;

        [Header("Animação")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _popupPanel;
        [SerializeField] private float _animationDuration = 0.25f;

        /// <summary>Callback quando "Continuar" é clicado.</summary>
        public event System.Action OnContinue;

        /// <summary>Callback quando "Reiniciar" é clicado.</summary>
        public event System.Action OnRestart;

        /// <summary>Callback quando "Menu" é clicado.</summary>
        public event System.Action OnMenu;

        private void Start()
        {
            UpdateLocalization();

            if (_continueButton != null)
                _continueButton.onClick.AddListener(HandleContinue);
            if (_restartButton != null)
                _restartButton.onClick.AddListener(HandleRestart);
            if (_menuButton != null)
                _menuButton.onClick.AddListener(HandleMenu);

            PlayEnterAnimation();
        }

        private void UpdateLocalization()
        {
            if (!ServiceLocator.TryGet<ILocalizationService>(out var loc))
            {
                // Fallback texts
                if (_titleText != null) _titleText.text = "Pausado";
                if (_continueButtonText != null) _continueButtonText.text = "Continuar";
                if (_restartButtonText != null) _restartButtonText.text = "Reiniciar";
                if (_menuButtonText != null) _menuButtonText.text = "Menu";
                return;
            }

            if (_titleText != null)
                _titleText.text = loc.Get("title_pause");
            if (_continueButtonText != null)
                _continueButtonText.text = loc.Get("btn_resume");
            if (_restartButtonText != null)
                _restartButtonText.text = loc.Get("btn_retry");
            if (_menuButtonText != null)
                _menuButtonText.text = loc.Get("btn_menu");
        }

        private void PlayEnterAnimation()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                StartCoroutine(AnimateIn());
            }

            if (_popupPanel != null)
                _popupPanel.localScale = Vector3.one * 0.8f;
        }

        private System.Collections.IEnumerator AnimateIn()
        {
            float elapsed = 0f;
            Vector3 startScale = Vector3.one * 0.8f;
            Vector3 endScale = Vector3.one;

            while (elapsed < _animationDuration)
            {
                elapsed += Time.unscaledDeltaTime; // unscaled because game is paused
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

        private System.Collections.IEnumerator AnimateOutAndDestroy(System.Action callback)
        {
            float elapsed = 0f;

            while (elapsed < _animationDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = 1f - Mathf.SmoothStep(0f, 1f, elapsed / _animationDuration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = t;
                if (_popupPanel != null)
                    _popupPanel.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);

                yield return null;
            }

            callback?.Invoke();
            Destroy(gameObject);
        }

        private void HandleContinue()
        {
            StartCoroutine(AnimateOutAndDestroy(() =>
            {
                Time.timeScale = 1f;
                GameManager.Instance?.StateMachine.TransitionTo(GameStateType.Playing);
                OnContinue?.Invoke();
            }));
        }

        private void HandleRestart()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.StateMachine.TransitionTo(GameStateType.Playing);
            OnRestart?.Invoke();

            // Recarregar cena Game para reiniciar
            GameManager.Instance?.LoadScene("Game");
            Destroy(gameObject);
        }

        private void HandleMenu()
        {
            Time.timeScale = 1f;

            var levelManager = GameManager.Instance?.LevelManager;
            if (levelManager != null)
                levelManager.QuitLevel(0);

            GameManager.Instance?.StateMachine.TransitionTo(GameStateType.LevelSelect);
            GameManager.Instance?.LoadScene("MainMenu");

            OnMenu?.Invoke();
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_continueButton != null)
                _continueButton.onClick.RemoveListener(HandleContinue);
            if (_restartButton != null)
                _restartButton.onClick.RemoveListener(HandleRestart);
            if (_menuButton != null)
                _menuButton.onClick.RemoveListener(HandleMenu);
        }
    }
}
