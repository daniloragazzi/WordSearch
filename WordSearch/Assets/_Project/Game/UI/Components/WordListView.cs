using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Domain.Grid;

namespace RagazziStudios.Game.UI.Components
{
    /// <summary>
    /// Lista visual das palavras a encontrar no nível.
    /// Mostra cada palavra com estado normal ou riscado (encontrado).
    /// </summary>
    public class WordListView : MonoBehaviour
    {
        [Header("Layout")]
        [SerializeField] private Transform _wordContainer;
        [SerializeField] private GameObject _wordItemPrefab;

        [Header("Visual")]
        [SerializeField] private Color _normalColor = new Color(0.2f, 0.2f, 0.2f);
        [SerializeField] private Color _foundColor = new Color(0.3f, 0.8f, 0.3f);
        [SerializeField] private float _foundFontScale = 0.95f;

        private readonly Dictionary<string, WordListItem> _items =
            new Dictionary<string, WordListItem>();

        /// <summary>
        /// Preenche a lista com as palavras do nível.
        /// </summary>
        public void SetWords(List<WordPlacement> placements)
        {
            ClearWords();

            foreach (var placement in placements)
            {
                var itemObj = Instantiate(_wordItemPrefab, _wordContainer);
                itemObj.SetActive(true);
                var item = itemObj.GetComponent<WordListItem>();

                if (item != null)
                {
                    item.Setup(placement.DisplayWord, _normalColor);
                    _items[placement.NormalizedWord] = item;
                }
            }
        }

        /// <summary>
        /// Marca uma palavra como encontrada (riscada).
        /// </summary>
        public void MarkWordFound(WordPlacement placement)
        {
            if (_items.TryGetValue(placement.NormalizedWord, out var item))
            {
                item.SetFound(_foundColor);
            }
        }

        /// <summary>
        /// Limpa todos os itens da lista.
        /// </summary>
        public void ClearWords()
        {
            foreach (Transform child in _wordContainer)
            {
                Destroy(child.gameObject);
            }
            _items.Clear();
        }
    }

    /// <summary>
    /// Item individual na lista de palavras.
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
                _wordText.fontStyle |= FontStyles.Strikethrough;
            }

            if (_strikethroughLine != null)
                _strikethroughLine.SetActive(true);
        }
    }
}
