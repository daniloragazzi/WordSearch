using UnityEngine;
using RagazziStudios.Core.Domain;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Vincula Camera.backgroundColor a um token de GameTheme.
    /// Re-aplica automaticamente ao trocar o tema via ThemeManager.OnThemeChanged.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class CameraThemeBinding : MonoBehaviour
    {
        [Tooltip("Token de cor do tema para o fundo da câmera.")]
        public ThemeColorRole role = ThemeColorRole.Background;

        private Camera _camera;

        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
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

        private void Apply(GameTheme theme)
        {
            if (theme == null || _camera == null) return;
            _camera.backgroundColor = Resolve(theme);
        }

        private Color Resolve(GameTheme t) => role switch
        {
            ThemeColorRole.Background => t.background,
            ThemeColorRole.Surface => t.surface,
            ThemeColorRole.Primary => t.primary,
            ThemeColorRole.PrimaryDark => t.primaryDark,
            _ => t.background
        };
    }
}
