using UnityEngine;
using UnityEngine.UI;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Tela do menu principal.
    /// Botões: Jogar (→ CategorySelect), Configurações (abre SettingsPopup).
    /// </summary>
    public class MainMenuScreen : MonoBehaviour
    {
        [Header("Botões")]
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _challengeButton;
        [SerializeField] private Button _settingsButton;

        [Header("Textos")]
        [SerializeField] private TMPro.TMP_Text _titleText;
        [SerializeField] private TMPro.TMP_Text _playButtonText;
        [SerializeField] private TMPro.TMP_Text _challengeButtonText;
        [SerializeField] private TMPro.TMP_Text _settingsButtonText;
        [SerializeField] private TMPro.TMP_Text _versionText;

        [Header("Referências")]
        [SerializeField] private GameObject _settingsPopupPrefab;
        [SerializeField] private Transform _popupParent;

        [Header("Animação")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeInDuration = 0.4f;

        private void OnEnable()
        {
            if (_playButton != null)
                _playButton.onClick.AddListener(OnPlayClicked);
            if (_challengeButton != null)
                _challengeButton.onClick.AddListener(OnChallengeClicked);
            if (_settingsButton != null)
                _settingsButton.onClick.AddListener(OnSettingsClicked);

            UpdateLocalization();
            PlayFadeIn();
        }

        private void OnDisable()
        {
            if (_playButton != null)
                _playButton.onClick.RemoveListener(OnPlayClicked);
            if (_challengeButton != null)
                _challengeButton.onClick.RemoveListener(OnChallengeClicked);
            if (_settingsButton != null)
                _settingsButton.onClick.RemoveListener(OnSettingsClicked);
        }

        private void UpdateLocalization()
        {
            if (!ServiceLocator.TryGet<ILocalizationService>(out var loc))
                return;

            if (_titleText != null)
                _titleText.text = loc.Get("app_name");

            if (_playButtonText != null)
                _playButtonText.text = loc.Get("btn_play");

            if (_settingsButtonText != null)
                _settingsButtonText.text = loc.Get("btn_settings");

            if (_challengeButtonText != null)
                _challengeButtonText.text = "Desafio";

            if (_versionText != null)
                _versionText.text = $"v{UnityEngine.Application.version}";
        }

        private void PlayFadeIn()
        {
            if (_canvasGroup == null) return;

            _canvasGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }

        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < _fadeInDuration)
            {
                elapsed += Time.deltaTime;
                _canvasGroup.alpha = Mathf.Clamp01(elapsed / _fadeInDuration);
                yield return null;
            }
            _canvasGroup.alpha = 1f;
        }

        private void OnPlayClicked()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.StateMachine.TransitionTo(GameStateType.CategorySelect);
        }

        private void OnChallengeClicked()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.StateMachine.TransitionTo(GameStateType.ChallengeSelect);
        }

        private void OnSettingsClicked()
        {
            if (_settingsPopupPrefab == null || _popupParent == null) return;

            var popup = Instantiate(_settingsPopupPrefab, _popupParent);
            popup.SetActive(true);
        }
    }
}
