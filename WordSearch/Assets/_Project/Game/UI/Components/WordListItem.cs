using UnityEngine;
using TMPro;

namespace RagazziStudios.Game.UI.Components
{
    /// <summary>
    /// Item individual na lista de palavras.
    /// Deve estar em arquivo próprio (Unity exige nome do arquivo = nome da classe MonoBehaviour).
    /// </summary>
    public class WordListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _wordText;
        [SerializeField] private GameObject _strikethroughLine;

        private bool _isFound;

        /// <summary>
        /// Configura o item com a palavra.
        /// </summary>
        public void Setup(string word, Color textColor)
        {
            if (_wordText != null)
            {
                _wordText.text = word;
                _wordText.color = textColor;
            }

            if (_strikethroughLine != null)
                _strikethroughLine.SetActive(false);

            _isFound = false;
        }

        /// <summary>
        /// Marca a palavra como encontrada com efeito riscado.
        /// </summary>
        public void SetFound(Color foundColor)
        {
            if (_isFound) return;

            _isFound = true;

            if (_wordText != null)
            {
                _wordText.color = foundColor;
                _wordText.fontStyle &= ~FontStyles.Strikethrough;
            }

            if (_strikethroughLine != null)
                _strikethroughLine.SetActive(true);
        }
    }
}
