using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Domain;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Papel semântico de cor no sistema de tema.
    /// Mapeado para tokens de GameTheme.
    /// </summary>
    public enum ThemeColorRole
    {
        Background,         // theme.background
        Surface,            // theme.surface
        Primary,            // theme.primary
        PrimaryDark,        // theme.primaryDark
        Accent,             // theme.accent
        TextPrimary,        // theme.textPrimary
        TextSecondary,      // theme.textSecondary
        TextOnColor,        // theme.textOnColor
        TextDisabled,       // theme.textDisabled
        GridBackground,     // theme.gridBackground
        CellNormal,         // theme.cellNormal
        ButtonPrimary,      // theme.buttonPrimary
        ButtonSecondary,    // theme.buttonSecondary
        ButtonDisabled,     // theme.buttonDisabled
        Success,            // theme.success
        Warning,            // theme.warning
        Error,              // theme.error
        Overlay,            // theme.overlay
    }

    /// <summary>
    /// Vincula a cor de um Graphic (Image) ou TMP_Text a um token de GameTheme.
    /// Re-aplica automaticamente ao trocar o tema via ThemeManager.OnThemeChanged.
    ///
    /// Uso: adicione este componente a um GameObject com Image ou TMP_Text,
    ///      configure o 'role' no Inspector.
    ///      SceneCreator também pode adicioná-lo programaticamente.
    /// </summary>
    [DisallowMultipleComponent]
    public class ThemeColorBinding : MonoBehaviour
    {
        [Tooltip("Token de cor do tema a ser aplicado a este elemento.")]
        public ThemeColorRole role = ThemeColorRole.Background;

        [Tooltip("Multiplicador de alfa (0–1). Útil para overrides de transparência.")]
        [Range(0f, 1f)]
        public float alphaOverride = 1f;

        [Tooltip("Se true, mantém o alfa original do token de tema (ignora alphaOverride).")]
        public bool keepThemeAlpha = false;

        // ─────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            ThemeManager.OnThemeChanged += OnThemeChanged;
            ApplyCurrent();
        }

        private void OnDisable()
        {
            ThemeManager.OnThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(GameTheme theme) => Apply(theme);

        private void ApplyCurrent()
        {
            if (ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
                Apply(ThemeManager.Instance.CurrentTheme);
        }

        // ─────────────────────────────────────────────────────────────

        private void Apply(GameTheme theme)
        {
            if (theme == null) return;

            Color c = Resolve(theme);

            if (!keepThemeAlpha)
                c.a = alphaOverride;

            // Tenta aplicar a Image primeiro, depois TMP_Text, depois qualquer Graphic.
            var img = GetComponent<Image>();
            if (img != null) { img.color = c; return; }

            var txt = GetComponent<TMP_Text>();
            if (txt != null) { txt.color = c; return; }

            var gfx = GetComponent<Graphic>();
            if (gfx != null) gfx.color = c;
        }

        private Color Resolve(GameTheme t) => role switch
        {
            ThemeColorRole.Background => t.background,
            ThemeColorRole.Surface => t.surface,
            ThemeColorRole.Primary => t.primary,
            ThemeColorRole.PrimaryDark => t.primaryDark,
            ThemeColorRole.Accent => t.accent,
            ThemeColorRole.TextPrimary => t.textPrimary,
            ThemeColorRole.TextSecondary => t.textSecondary,
            ThemeColorRole.TextOnColor => t.textOnColor,
            ThemeColorRole.TextDisabled => t.textDisabled,
            ThemeColorRole.GridBackground => t.gridBackground,
            ThemeColorRole.CellNormal => t.cellNormal,
            ThemeColorRole.ButtonPrimary => t.buttonPrimary,
            ThemeColorRole.ButtonSecondary => t.buttonSecondary,
            ThemeColorRole.ButtonDisabled => t.buttonDisabled,
            ThemeColorRole.Success => t.success,
            ThemeColorRole.Warning => t.warning,
            ThemeColorRole.Error => t.error,
            ThemeColorRole.Overlay => t.overlay,
            _ => t.background
        };
    }
}
