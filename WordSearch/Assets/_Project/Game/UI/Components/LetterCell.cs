using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

        [Header("Cores")]
        [SerializeField] private Color _normalColor = new Color(0.95f, 0.95f, 0.95f);
        [SerializeField] private Color _selectedColor = new Color(0.4f, 0.7f, 1f);
        [SerializeField] private Color _foundColor = new Color(0.3f, 0.85f, 0.4f);
        [SerializeField] private Color _hintColor = new Color(1f, 0.85f, 0.3f);
        [SerializeField] private Color _normalTextColor = new Color(0.15f, 0.15f, 0.15f);
        [SerializeField] private Color _activeTextColor = Color.white;

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
                    break;

                case CellState.Found:
                    IsSelected = false;
                    IsFound = true;
                    SetColors(_foundColor, _activeTextColor);
                    break;

                case CellState.Hint:
                    SetColors(_hintColor, _activeTextColor);
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
