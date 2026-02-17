using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Domain.Words;
using RagazziStudios.Game.Config;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Item visual de uma categoria na tela de seleção.
    /// Mostra ícone (emoji), nome e barra de progresso.
    /// Cor única por categoria via paleta.
    /// </summary>
    public class CategoryButtonItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _iconText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Image _progressFill;
        [SerializeField] private Image _iconImage;

        [Header("Tema")]
        [SerializeField] private GameTheme _theme;

        private string _categoryId;
        private Action<string> _onClickCallback;

        /// <summary>Paleta de cores por categoria (ordem fixa).</summary>
        private static readonly System.Collections.Generic.Dictionary<string, Color> CategoryColors =
            new System.Collections.Generic.Dictionary<string, Color>
        {
            { "animais",      new Color(0.30f, 0.55f, 0.35f) }, // verde escuro
            { "alimentos",    new Color(0.75f, 0.40f, 0.20f) }, // laranja terra
            { "corpo_humano", new Color(0.65f, 0.25f, 0.30f) }, // vermelho suave
            { "natureza",     new Color(0.25f, 0.50f, 0.45f) }, // teal
            { "profissoes",   new Color(0.50f, 0.35f, 0.60f) }, // roxo
            { "paises",       new Color(0.25f, 0.40f, 0.65f) }, // azul
            { "esportes",     new Color(0.60f, 0.50f, 0.20f) }, // dourado
            { "cores_formas", new Color(0.55f, 0.30f, 0.50f) }, // magenta
        };

        /// <summary>
        /// Configura o item da categoria.
        /// </summary>
        public void Setup(CategoryData category, int completedLevels, int totalLevels,
            Action<string> onClickCallback)
        {
            _categoryId = category.id;
            _onClickCallback = onClickCallback;

            // Aplicar cor da categoria ao fundo do card
            ApplyCategoryColor(category.id);

            // Aplicar ícone (sprite se disponível, senão emoji)
            ApplyCategoryIcon(category);

            // Esconder ícone-texto se vazio e expandir nome/progresso
            bool hasIcon = _iconImage != null || !string.IsNullOrEmpty(category.icon);
            if (_iconText != null)
            {
                _iconText.text = hasIcon ? category.icon : "";
                _iconText.gameObject.SetActive(hasIcon);
            }

            if (_nameText != null)
            {
                _nameText.text = category.name;
                // Se não tem ícone, expandir nome para usar toda a largura
                if (!hasIcon)
                {
                    var nameRect = _nameText.GetComponent<RectTransform>();
                    if (nameRect != null)
                    {
                        nameRect.anchorMin = new Vector2(0.05f, nameRect.anchorMin.y);
                    }
                }
            }

            if (_progressText != null)
            {
                _progressText.text = $"{completedLevels}/{totalLevels}";
                if (!hasIcon)
                {
                    var progRect = _progressText.GetComponent<RectTransform>();
                    if (progRect != null)
                    {
                        progRect.anchorMin = new Vector2(0.05f, progRect.anchorMin.y);
                    }
                }
            }

            if (_progressFill != null)
            {
                float progress = totalLevels > 0
                    ? (float)completedLevels / totalLevels
                    : 0f;
                _progressFill.fillAmount = progress;
            }

            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            _onClickCallback?.Invoke(_categoryId);
        }

        private void ApplyCategoryColor(string categoryId)
        {
            var bgImage = GetComponent<Image>();
            if (bgImage == null) return;

            if (_theme != null)
            {
                bgImage.color = _theme.GetCategoryColor(categoryId);
                return;
            }

            if (CategoryColors.TryGetValue(categoryId, out var color))
            {
                bgImage.color = color;
            }
        }

        private void ApplyCategoryIcon(CategoryData category)
        {
            // Tentar carregar sprite do ícone da categoria
            string spritePath = $"CategoryIcons/cat_{category.id}";
            var sprite = Resources.Load<Sprite>(spritePath);

            if (sprite != null && _iconImage != null)
            {
                _iconImage.sprite = sprite;
                _iconImage.gameObject.SetActive(true);
                if (_iconText != null)
                    _iconText.gameObject.SetActive(false);
            }
            else if (_iconText != null && !string.IsNullOrEmpty(category.icon))
            {
                _iconText.text = category.icon;
                _iconText.gameObject.SetActive(true);
            }
        }
    }
}
