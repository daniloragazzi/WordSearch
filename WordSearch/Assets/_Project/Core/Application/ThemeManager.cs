using System;
using UnityEngine;
using RagazziStudios.Core.Infrastructure.Storage;
using RagazziStudios.Game.Config;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Modos de tema disponíveis.
    /// </summary>
    public enum ThemeMode
    {
        System = 0,  // Segue preferência do sistema operacional
        Light = 1,  // Sempre tema claro
        Dark = 2   // Sempre tema escuro
    }

    /// <summary>
    /// Singleton responsável pelo tema visual do jogo.
    /// Persiste entre cenas (DontDestroyOnLoad).
    /// Detecta preferência do sistema Android e persiste escolha do usuário.
    /// Referências de assets: Resources/Themes/GameTheme_Light e GameTheme_Dark.
    /// </summary>
    public class ThemeManager : MonoBehaviour
    {
        public static ThemeManager Instance { get; private set; }

        // ─────────────────────────────────────────────────────────────
        //  Eventos
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Disparado quando o tema ativo é alterado.
        /// Assine para re-colorir componentes em runtime.
        /// </summary>
        public static event Action<GameTheme> OnThemeChanged;

        // ─────────────────────────────────────────────────────────────
        //  Estado
        // ─────────────────────────────────────────────────────────────

        private GameTheme _lightTheme;
        private GameTheme _darkTheme;
        private ThemeMode _currentMode = ThemeMode.System;

        // ─────────────────────────────────────────────────────────────
        //  Propriedades públicas
        // ─────────────────────────────────────────────────────────────

        /// <summary>Tema atualmente ativo (claro ou escuro).</summary>
        public GameTheme CurrentTheme { get; private set; }

        /// <summary>Modo selecionado pelo usuário.</summary>
        public ThemeMode CurrentMode => _currentMode;

        /// <summary>Atalho: o tema ativo é escuro?</summary>
        public bool IsDarkActive => CurrentTheme != null && CurrentTheme.isDark;

        // ─────────────────────────────────────────────────────────────
        //  Unity Lifecycle
        // ─────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadThemeAssets();
            LoadSavedMode();
            ApplyTheme(silent: true);
        }

        // ─────────────────────────────────────────────────────────────
        //  API pública
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Altera o modo de tema e persiste a escolha.
        /// Dispara OnThemeChanged com o novo GameTheme.
        /// </summary>
        public void SetThemeMode(ThemeMode mode)
        {
            _currentMode = mode;
            SaveMode();
            ApplyTheme(silent: false);
        }

        /// <summary>
        /// Reaplica o tema atual (útil após carregamento de nova cena).
        /// </summary>
        public void Refresh()
        {
            ApplyTheme(silent: false);
        }

        // ─────────────────────────────────────────────────────────────
        //  Internos
        // ─────────────────────────────────────────────────────────────

        private void LoadThemeAssets()
        {
            _lightTheme = Resources.Load<GameTheme>("Themes/GameTheme_Light");
            _darkTheme = Resources.Load<GameTheme>("Themes/GameTheme_Dark");

            if (_lightTheme == null)
                Debug.LogWarning("[ThemeManager] GameTheme_Light não encontrado em Resources/Themes/. Execute 'Build → Ragazzi Studios → Generate Theme Assets' no Unity.");
            if (_darkTheme == null)
                Debug.LogWarning("[ThemeManager] GameTheme_Dark não encontrado em Resources/Themes/. Execute 'Build → Ragazzi Studios → Generate Theme Assets' no Unity.");
        }

        private void LoadSavedMode()
        {
            string saved = PlayerPrefs.GetString(StorageKeys.THEME_MODE, "system");
            _currentMode = saved switch
            {
                "light" => ThemeMode.Light,
                "dark" => ThemeMode.Dark,
                _ => ThemeMode.System
            };
        }

        private void SaveMode()
        {
            string value = _currentMode switch
            {
                ThemeMode.Light => "light",
                ThemeMode.Dark => "dark",
                _ => "system"
            };
            PlayerPrefs.SetString(StorageKeys.THEME_MODE, value);
            PlayerPrefs.Save();
        }

        private void ApplyTheme(bool silent)
        {
            bool useDark = _currentMode switch
            {
                ThemeMode.Dark => true,
                ThemeMode.Light => false,
                _ => IsSystemDark()
            };

            GameTheme next = useDark ? _darkTheme : _lightTheme;

            // Fallback: se um dos assets não existir, usa o outro
            if (next == null)
            {
                next = _lightTheme ?? _darkTheme;
                Debug.LogWarning("[ThemeManager] Asset de tema não disponível — usando fallback.");
            }

            CurrentTheme = next;

            if (!silent)
            {
                OnThemeChanged?.Invoke(CurrentTheme);
            }

            Debug.Log($"[ThemeManager] Tema ativo: {(useDark ? "Dark" : "Light")} (modo: {_currentMode})");
        }

        /// <summary>
        /// Detecta se o sistema operacional está no modo escuro.
        /// Android: lê Configuration.uiMode via JNI.
        /// Outros: retorna false (tema claro).
        /// </summary>
        private bool IsSystemDark()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                using var player   = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = player.GetStatic<AndroidJavaObject>("currentActivity");
                using var resources= activity.Call<AndroidJavaObject>("getResources");
                using var config   = resources.Call<AndroidJavaObject>("getConfiguration");

                int uiMode = config.Get<int>("uiMode");
                const int UI_MODE_NIGHT_MASK = 0x30;
                const int UI_MODE_NIGHT_YES  = 0x20;
                return (uiMode & UI_MODE_NIGHT_MASK) == UI_MODE_NIGHT_YES;
            }
            catch (Exception e)
            {
                Debug.LogWarning("[ThemeManager] Falha ao detectar tema do sistema: " + e.Message);
                return false;
            }
#else
            return false;
#endif
        }
    }
}
