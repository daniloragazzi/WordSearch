using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Domain.Words;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Item visual de uma categoria na tela de seleção.
    /// Mostra ícone (emoji), nome e barra de progresso.
    /// </summary>
    public class CategoryButtonItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _iconText;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private Image _progressFill;

        private string _categoryId;
        private Action<string> _onClickCallback;

        /// <summary>
        /// Configura o item da categoria.
        /// </summary>
        public void Setup(CategoryData category, int completedLevels, int totalLevels,
            Action<string> onClickCallback)
        {
            _categoryId = category.id;
            _onClickCallback = onClickCallback;

            // Esconder ícone se vazio e expandir nome/progresso
            bool hasIcon = !string.IsNullOrEmpty(category.icon);
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
    }
}
