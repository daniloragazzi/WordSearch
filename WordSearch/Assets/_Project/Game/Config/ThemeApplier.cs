using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RagazziStudios.Game.Config
{
    /// <summary>
    /// Componente que aplica o GameTheme a elementos UI do GameObject.
    /// Attach em qualquer UI element para colorir automaticamente
    /// de acordo com o tema configurado.
    /// </summary>
    public class ThemeApplier : MonoBehaviour
    {
        [Header("Tema")]
        [SerializeField] private GameTheme _theme;

        [Header("Tipo do Elemento")]
        [SerializeField] private ThemeElement _elementType = ThemeElement.None;

        [Header("Alvos (auto-detecta se vazio)")]
        [SerializeField] private Image _targetImage;
        [SerializeField] private TMP_Text _targetText;

        private void Start()
        {
            if (_theme == null)
            {
                Debug.LogWarning($"[ThemeApplier] No theme assigned on {gameObject.name}");
                return;
            }

            AutoDetectTargets();
            ApplyTheme();
        }

        private void AutoDetectTargets()
        {
            if (_targetImage == null)
                _targetImage = GetComponent<Image>();

            if (_targetText == null)
                _targetText = GetComponent<TMP_Text>();
        }

        /// <summary>
        /// Aplica as cores do tema ao elemento.
        /// </summary>
        public void ApplyTheme()
        {
            if (_theme == null) return;

            switch (_elementType)
            {
                // Fundos
                case ThemeElement.Background:
                    SetImageColor(_theme.background);
                    break;
                case ThemeElement.Surface:
                    SetImageColor(_theme.surface);
                    break;
                case ThemeElement.Overlay:
                    SetImageColor(_theme.overlay);
                    break;
                case ThemeElement.GridBackground:
                    SetImageColor(_theme.gridBackground);
                    break;

                // Textos
                case ThemeElement.TextPrimary:
                    SetTextColor(_theme.textPrimary);
                    break;
                case ThemeElement.TextSecondary:
                    SetTextColor(_theme.textSecondary);
                    break;
                case ThemeElement.TextOnColor:
                    SetTextColor(_theme.textOnColor);
                    break;
                case ThemeElement.TextDisabled:
                    SetTextColor(_theme.textDisabled);
                    break;

                // Botões
                case ThemeElement.ButtonPrimary:
                    SetImageColor(_theme.buttonPrimary);
                    SetTextColor(_theme.textOnColor);
                    break;
                case ThemeElement.ButtonSecondary:
                    SetImageColor(_theme.buttonSecondary);
                    SetTextColor(_theme.textPrimary);
                    break;
                case ThemeElement.ButtonDisabled:
                    SetImageColor(_theme.buttonDisabled);
                    SetTextColor(_theme.textDisabled);
                    break;

                // Grid
                case ThemeElement.CellNormal:
                    SetImageColor(_theme.cellNormal);
                    SetTextColor(_theme.cellLetterNormal);
                    break;
                case ThemeElement.CellSelected:
                    SetImageColor(_theme.cellSelected);
                    SetTextColor(_theme.cellLetterActive);
                    break;
                case ThemeElement.CellFound:
                    SetImageColor(_theme.cellFound);
                    SetTextColor(_theme.cellLetterActive);
                    break;
                case ThemeElement.CellHint:
                    SetImageColor(_theme.cellHint);
                    SetTextColor(_theme.cellLetterActive);
                    break;

                // Accent
                case ThemeElement.Primary:
                    SetImageColor(_theme.primary);
                    break;
                case ThemeElement.Accent:
                    SetImageColor(_theme.accent);
                    break;
                case ThemeElement.Success:
                    SetImageColor(_theme.success);
                    break;
                case ThemeElement.Warning:
                    SetImageColor(_theme.warning);
                    break;
                case ThemeElement.Error:
                    SetImageColor(_theme.error);
                    break;

                case ThemeElement.None:
                default:
                    break;
            }
        }

        private void SetImageColor(Color color)
        {
            if (_targetImage != null)
                _targetImage.color = color;
        }

        private void SetTextColor(Color color)
        {
            if (_targetText != null)
                _targetText.color = color;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Preview no editor ao mudar valores.
        /// </summary>
        private void OnValidate()
        {
            if (_theme != null && Application.isPlaying)
            {
                AutoDetectTargets();
                ApplyTheme();
            }
        }
#endif
    }

    /// <summary>
    /// Tipos de elementos temáticos disponíveis.
    /// </summary>
    public enum ThemeElement
    {
        None,

        // Fundos
        Background,
        Surface,
        Overlay,
        GridBackground,

        // Textos
        TextPrimary,
        TextSecondary,
        TextOnColor,
        TextDisabled,

        // Botões
        ButtonPrimary,
        ButtonSecondary,
        ButtonDisabled,

        // Grid
        CellNormal,
        CellSelected,
        CellFound,
        CellHint,

        // Cores semânticas
        Primary,
        Accent,
        Success,
        Warning,
        Error
    }
}
