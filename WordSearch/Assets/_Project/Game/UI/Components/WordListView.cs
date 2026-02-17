using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Domain.Grid;
using RagazziStudios.Core.Domain;
using RagazziStudios.Core.Application;

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

        [Header("Tema")]
        [SerializeField] private GameTheme _theme;

        [Header("Visual")]
        [SerializeField] private Color _normalColor = new Color(0.20f, 0.22f, 0.28f);
        [SerializeField] private Color _foundColor = new Color(0.30f, 0.80f, 0.40f);

        private readonly Dictionary<string, WordListItem> _items =
            new Dictionary<string, WordListItem>();

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
            RecolorExistingItems();
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

            _normalColor = theme.wordNormal;
            _foundColor = theme.wordFound;
        }

        private void RecolorExistingItems()
        {
            foreach (var kvp in _items)
            {
                var item = kvp.Value;
                if (item != null)
                    item.Setup(item.Word, _normalColor);
            }
        }

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
}
