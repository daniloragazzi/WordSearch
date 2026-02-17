using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Localization;
using RagazziStudios.Core.Infrastructure.Storage;
using System.Collections.Generic;

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
        [SerializeField] private TMP_Text _themeLabel;

        [Header("Controles")]
        [SerializeField] private Toggle _soundToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [SerializeField] private TMP_Dropdown _themeDropdown;

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

            ApplyResponsiveLayout();
            InitializeControls();
            UpdateLocalization();
            PlayEnterAnimation();
        }

        private void ApplyResponsiveLayout()
        {
            if (_titleText != null)
            {
                _titleText.alignment = TextAlignmentOptions.Center;
                SetAnchors(_titleText.rectTransform, new Vector2(0.15f, 0.70f), new Vector2(0.85f, 0.88f));
            }

            // Row 1 — Som
            if (_soundLabel != null)
            {
                _soundLabel.alignment = TextAlignmentOptions.MidlineLeft;
                SetAnchors(_soundLabel.rectTransform, new Vector2(0.08f, 0.56f), new Vector2(0.45f, 0.67f));
            }
            if (_soundToggle != null)
            {
                var rt = _soundToggle.GetComponent<RectTransform>();
                SetAnchors(rt, new Vector2(0.68f, 0.56f), new Vector2(0.86f, 0.67f));
            }

            // Row 2 — Música
            if (_musicLabel != null)
            {
                _musicLabel.alignment = TextAlignmentOptions.MidlineLeft;
                SetAnchors(_musicLabel.rectTransform, new Vector2(0.08f, 0.44f), new Vector2(0.45f, 0.55f));
            }
            if (_musicToggle != null)
            {
                var rt = _musicToggle.GetComponent<RectTransform>();
                SetAnchors(rt, new Vector2(0.68f, 0.44f), new Vector2(0.86f, 0.55f));
            }

            // Row 3 — Idioma
            if (_languageLabel != null)
            {
                _languageLabel.alignment = TextAlignmentOptions.MidlineLeft;
                SetAnchors(_languageLabel.rectTransform, new Vector2(0.08f, 0.31f), new Vector2(0.45f, 0.42f));
            }
            if (_languageDropdown != null)
            {
                var rt = _languageDropdown.GetComponent<RectTransform>();
                SetAnchors(rt, new Vector2(0.50f, 0.30f), new Vector2(0.92f, 0.43f));
            }

            // Row 4 — Tema
            if (_themeLabel != null)
            {
                _themeLabel.alignment = TextAlignmentOptions.MidlineLeft;
                SetAnchors(_themeLabel.rectTransform, new Vector2(0.08f, 0.17f), new Vector2(0.45f, 0.28f));
            }
            if (_themeDropdown != null)
            {
                var rt = _themeDropdown.GetComponent<RectTransform>();
                SetAnchors(rt, new Vector2(0.50f, 0.16f), new Vector2(0.92f, 0.29f));
            }

            if (_closeButton != null)
            {
                var rt = _closeButton.GetComponent<RectTransform>();
                SetAnchors(rt, new Vector2(0.82f, 0.76f), new Vector2(0.94f, 0.90f));
            }
        }

        private static void SetAnchors(RectTransform rectTransform, Vector2 anchorMin, Vector2 anchorMax)
        {
            if (rectTransform == null) return;

            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
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

            // Tema
            if (_themeDropdown != null)
            {
                _themeDropdown.ClearOptions();
                _themeDropdown.AddOptions(new List<string> { "Sistema", "Claro", "Escuro" });

                if (ThemeManager.Instance != null)
                    _themeDropdown.value = (int)ThemeManager.Instance.CurrentMode;

                _themeDropdown.onValueChanged.AddListener(OnThemeDropdownChanged);
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

            if (_themeLabel != null)
                _themeLabel.text = "Tema";
        }

        // --- Callbacks ---

        private void OnSoundChanged(bool enabled)
        {
            if (_storage == null) return;

            _storage.SetBool(StorageKeys.SOUND_ENABLED, enabled);
            _storage.Save();

            // SFX controlled per-PlayOneShot via IsSfxEnabled() check
            Debug.Log($"[Settings] Sound: {(enabled ? "ON" : "OFF")}");
        }

        private void OnMusicChanged(bool enabled)
        {
            if (_storage == null) return;

            _storage.SetBool(StorageKeys.MUSIC_ENABLED, enabled);
            _storage.Save();

            // Controlar MusicManager diretamente
            if (MusicManager.Instance != null)
                MusicManager.Instance.SetEnabled(enabled);

            Debug.Log($"[Settings] Music: {(enabled ? "ON" : "OFF")}");
        }

        private void OnThemeDropdownChanged(int index)
        {
            var mode = (ThemeMode)index;
            if (ThemeManager.Instance != null)
                ThemeManager.Instance.SetThemeMode(mode);

            Debug.Log($"[Settings] Theme: {mode}");
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

            if (_themeDropdown != null)
                _themeDropdown.onValueChanged.RemoveListener(OnThemeDropdownChanged);

            if (_closeButton != null)
                _closeButton.onClick.RemoveListener(OnCloseClicked);
        }
    }
}
