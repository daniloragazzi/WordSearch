using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;
using RagazziStudios.Core.Infrastructure.Storage;

namespace RagazziStudios.Game.UI.Popups
{
    /// <summary>
    /// Popup de configurações.
    /// Controles: som on/off, música on/off, seleção de idioma.
    /// </summary>
    public class SettingsPopup : MonoBehaviour
    {
        [Header("Textos")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _soundLabel;
        [SerializeField] private TMP_Text _musicLabel;
        [SerializeField] private TMP_Text _languageLabel;

        [Header("Controles")]
        [SerializeField] private Toggle _soundToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private TMP_Dropdown _languageDropdown;

        [Header("Botões")]
        [SerializeField] private Button _closeButton;

        [Header("Animação")]
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _popupPanel;
        [SerializeField] private float _animationDuration = 0.25f;

        private IStorageService _storage;
        private ILocalizationService _localization;

        private readonly string[] _supportedLanguages = { "pt-BR" };
        private readonly string[] _languageNames = { "Português (BR)" };

        private void Start()
        {
            ServiceLocator.TryGet(out _storage);
            ServiceLocator.TryGet(out _localization);

            InitializeControls();
            UpdateLocalization();
            PlayEnterAnimation();
        }

        private void InitializeControls()
        {
            // Som
            if (_soundToggle != null && _storage != null)
            {
                _soundToggle.isOn = _storage.GetBool(StorageKeys.SOUND_ENABLED, true);
                _soundToggle.onValueChanged.AddListener(OnSoundChanged);
            }

            // Música
            if (_musicToggle != null && _storage != null)
            {
                _musicToggle.isOn = _storage.GetBool(StorageKeys.MUSIC_ENABLED, true);
                _musicToggle.onValueChanged.AddListener(OnMusicChanged);
            }

            // Idioma
            if (_languageDropdown != null)
            {
                _languageDropdown.ClearOptions();
                _languageDropdown.AddOptions(
                    new System.Collections.Generic.List<string>(_languageNames));

                // Selecionar idioma atual
                if (_localization != null)
                {
                    int idx = System.Array.IndexOf(
                        _supportedLanguages, _localization.CurrentLanguage);
                    _languageDropdown.value = Mathf.Max(0, idx);
                }

                _languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
            }

            // Fechar
            if (_closeButton != null)
                _closeButton.onClick.AddListener(OnCloseClicked);
        }

        private void UpdateLocalization()
        {
            if (_localization == null) return;

            if (_titleText != null)
                _titleText.text = _localization.Get("title_settings");

            if (_soundLabel != null)
                _soundLabel.text = _localization.Get("settings_sound");

            if (_musicLabel != null)
                _musicLabel.text = _localization.Get("settings_music");

            if (_languageLabel != null)
                _languageLabel.text = _localization.Get("settings_language");
        }

        // --- Callbacks ---

        private void OnSoundChanged(bool enabled)
        {
            if (_storage == null) return;

            _storage.SetBool(StorageKeys.SOUND_ENABLED, enabled);
            _storage.Save();

            // Aplicar volume de SFX globalmente
            AudioListener.volume = enabled ? 1f : 0f;

            Debug.Log($"[Settings] Sound: {(enabled ? "ON" : "OFF")}");
        }

        private void OnMusicChanged(bool enabled)
        {
            if (_storage == null) return;

            _storage.SetBool(StorageKeys.MUSIC_ENABLED, enabled);
            _storage.Save();

            Debug.Log($"[Settings] Music: {(enabled ? "ON" : "OFF")}");
        }

        private void OnLanguageChanged(int index)
        {
            if (index < 0 || index >= _supportedLanguages.Length) return;

            string langCode = _supportedLanguages[index];

            if (_localization != null)
                _localization.SetLanguage(langCode);

            if (_storage != null)
            {
                _storage.SetString(StorageKeys.LANGUAGE, langCode);
                _storage.Save();
            }

            UpdateLocalization();

            Debug.Log($"[Settings] Language: {langCode}");
        }

        private void OnCloseClicked()
        {
            PlayExitAnimation();
        }

        // --- Animação ---

        private void PlayEnterAnimation()
        {
            if (_canvasGroup == null) return;

            _canvasGroup.alpha = 0f;
            if (_popupPanel != null) _popupPanel.localScale = Vector3.one * 0.85f;

            StartCoroutine(AnimatePopup(0f, 1f, Vector3.one * 0.85f, Vector3.one));
        }

        private void PlayExitAnimation()
        {
            if (_canvasGroup == null)
            {
                Destroy(gameObject);
                return;
            }

            StartCoroutine(AnimateAndDestroy());
        }

        private System.Collections.IEnumerator AnimatePopup(
            float fromAlpha, float toAlpha,
            Vector3 fromScale, Vector3 toScale)
        {
            float elapsed = 0f;
            while (elapsed < _animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / _animationDuration);

                if (_canvasGroup != null)
                    _canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, t);

                if (_popupPanel != null)
                    _popupPanel.localScale = Vector3.Lerp(fromScale, toScale, t);

                yield return null;
            }

            if (_canvasGroup != null) _canvasGroup.alpha = toAlpha;
            if (_popupPanel != null) _popupPanel.localScale = toScale;
        }

        private System.Collections.IEnumerator AnimateAndDestroy()
        {
            yield return AnimatePopup(1f, 0f, Vector3.one, Vector3.one * 0.85f);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_soundToggle != null)
                _soundToggle.onValueChanged.RemoveListener(OnSoundChanged);

            if (_musicToggle != null)
                _musicToggle.onValueChanged.RemoveListener(OnMusicChanged);

            if (_languageDropdown != null)
                _languageDropdown.onValueChanged.RemoveListener(OnLanguageChanged);

            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseClicked);
        }
    }
}
