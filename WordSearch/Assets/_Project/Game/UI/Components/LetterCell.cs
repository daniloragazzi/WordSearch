using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Domain;
using RagazziStudios.Core.Application;

namespace RagazziStudios.Game.UI.Components
{
    /// <summary>
    /// Célula visual individual do grid de letras.
    /// Gerencia estados visuais: normal, selecionado, encontrado, dica.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class LetterCell : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_Text _letterText;
        [SerializeField] private Image _backgroundImage;

        [Header("Tema")]
        [SerializeField] private GameTheme _theme;

        [Header("Cores")]
        [SerializeField] private Color _normalColor = new Color(0.95f, 0.95f, 0.95f);
        [SerializeField] private Color _selectedColor = new Color(0.4f, 0.7f, 1f);
        [SerializeField] private Color _foundColor = new Color(0.3f, 0.85f, 0.4f);
        [SerializeField] private Color _hintColor = new Color(1f, 0.85f, 0.3f);
        [SerializeField] private Color _normalTextColor = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color _activeTextColor = Color.white;
        [SerializeField] private Color _hintTextColor = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color _invalidColor = new Color(1f, 0.3f, 0.3f, 0.85f);

        /// <summary>Estado visual atual da célula.</summary>
        private CellState _currentState = CellState.Normal;

        private void Awake()
        {
            ApplyThemeColors();
        }

        private void OnEnable()
        {
            ThemeManager.OnThemeChanged += OnThemeChanged;
        }

        private void OnDisable()
        {
            ThemeManager.OnThemeChanged -= OnThemeChanged;
        }

        private void OnThemeChanged(GameTheme newTheme)
        {
            ApplyThemeColors();
            // Re-apply current visual state with new colors
            SetState(_currentState);
        }

        private GameTheme ResolveTheme()
        {
            if (_theme != null) return _theme;
            return ThemeManager.Instance != null ? ThemeManager.Instance.CurrentTheme : null;
        }

        private void ApplyThemeColors()
        {
            var theme = ResolveTheme();
            if (theme == null) return;

            _normalColor = theme.cellNormal;
            _selectedColor = theme.cellSelected;
            _foundColor = theme.cellFound;
            _hintColor = theme.cellHint;
            _normalTextColor = theme.cellLetterNormal;
            _activeTextColor = theme.cellLetterActive;
            _hintTextColor = theme.GetContrastText(_hintColor);
            _invalidColor = new Color(theme.error.r, theme.error.g, theme.error.b, 0.85f);
        }

        /// <summary>Posição no grid (row, col).</summary>
        public int Row { get; private set; }
        public int Col { get; private set; }

        /// <summary>Letra exibida.</summary>
        public char Letter { get; private set; }

        /// <summary>Se esta célula faz parte de uma palavra encontrada.</summary>
        public bool IsFound { get; private set; }

        /// <summary>Se está atualmente selecionada pelo toque.</summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// Inicializa a célula com posição e letra.
        /// </summary>
        public void Setup(int row, int col, char letter)
        {
            Row = row;
            Col = col;
            Letter = letter;

            if (_letterText != null)
                _letterText.text = letter.ToString();

            SetState(CellState.Normal);
        }

        /// <summary>
        /// Define o estado visual da célula.
        /// </summary>
        public void SetState(CellState state)
        {
            _currentState = state;

            switch (state)
            {
                case CellState.Normal:
                    IsSelected = false;
                    if (!IsFound)
                    {
                        SetColors(_normalColor, _normalTextColor);
                    }
                    break;

                case CellState.Selected:
                    IsSelected = true;
                    SetColors(_selectedColor, _activeTextColor);
                    PopSelect();
                    break;

                case CellState.Found:
                    IsSelected = false;
                    IsFound = true;
                    SetColors(_foundColor, _activeTextColor);
                    break;

                case CellState.Hint:
                    SetColors(_hintColor, _hintTextColor);
                    break;
            }
        }

        private void SetColors(Color background, Color text)
        {
            if (_backgroundImage != null)
                _backgroundImage.color = background;

            if (_letterText != null)
                _letterText.color = text;
        }

        /// <summary>
        /// Micro-animação de "pop" ao selecionar (scale 1→1.15→1, rápido).
        /// </summary>
        private void PopSelect()
        {
            StopCoroutine(nameof(PopCoroutine));
            StartCoroutine(PopCoroutine());
        }

        private IEnumerator PopCoroutine()
        {
            var rt = GetComponent<RectTransform>();
            Vector3 orig = Vector3.one;
            Vector3 big = orig * 1.15f;

            float half = 0.06f;
            float elapsed = 0f;

            while (elapsed < half)
            {
                rt.localScale = Vector3.Lerp(orig, big, elapsed / half);
                elapsed += Time.deltaTime;
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < half)
            {
                rt.localScale = Vector3.Lerp(big, orig, elapsed / half);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rt.localScale = orig;
        }

        /// <summary>
        /// Pisca a célula em vermelho brevemente (seleção inválida).
        /// </summary>
        public void FlashInvalid(float duration = 0.3f)
        {
            if (!IsFound)
                StartCoroutine(FlashInvalidCoroutine(duration));
        }

        private IEnumerator FlashInvalidCoroutine(float duration)
        {
            SetColors(_invalidColor, _activeTextColor);

            yield return new WaitForSeconds(duration);

            if (!IsFound && !IsSelected)
                SetColors(_normalColor, _normalTextColor);
        }

        /// <summary>
        /// Animação de pulso (escala) ao encontrar palavra.
        /// </summary>
        public void PulseFound(float delay = 0f)
        {
            StartCoroutine(PulseCoroutine(delay));
        }

        private IEnumerator PulseCoroutine(float delay)
        {
            if (delay > 0f)
                yield return new WaitForSeconds(delay);

            var rt = GetComponent<RectTransform>();
            Vector3 orig = rt.localScale;
            Vector3 big = orig * 1.25f;

            // Crescer
            float half = 0.12f;
            float elapsed = 0f;
            while (elapsed < half)
            {
                float t = elapsed / half;
                rt.localScale = Vector3.Lerp(orig, big, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            // Encolher de volta
            elapsed = 0f;
            while (elapsed < half)
            {
                float t = elapsed / half;
                rt.localScale = Vector3.Lerp(big, orig, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            rt.localScale = orig;
        }

        /// <summary>
        /// Reseta completamente a célula.
        /// </summary>
        public void ResetCell()
        {
            IsFound = false;
            IsSelected = false;
            SetState(CellState.Normal);
        }
    }

    /// <summary>
    /// Estados visuais possíveis de uma célula.
    /// </summary>
    public enum CellState
    {
        Normal,
        Selected,
        Found,
        Hint
    }
}
