using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Item visual de um nível na tela de seleção.
    /// Exibe número, estado (completo/desbloqueado/bloqueado) e ícone de lock.
    /// </summary>
    public class LevelButtonItem : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private GameObject _completedIcon;
        [SerializeField] private GameObject _lockedIcon;

        private int _levelNumber;
        private Action<int> _onClickCallback;

        /// <summary>
        /// Configura o item do nível.
        /// </summary>
        public void Setup(int levelNumber, bool completed, bool unlocked,
            Color backgroundColor, Color textColor, Action<int> onClickCallback)
        {
            _levelNumber = levelNumber;
            _onClickCallback = onClickCallback;

            if (_numberText != null)
            {
                _numberText.text = levelNumber.ToString();
                _numberText.color = textColor;
            }

            if (_backgroundImage != null)
                _backgroundImage.color = backgroundColor;

            if (_completedIcon != null)
                _completedIcon.SetActive(completed);

            if (_lockedIcon != null)
                _lockedIcon.SetActive(!unlocked);

            _button.interactable = unlocked;
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        private void OnClicked()
        {
            _onClickCallback?.Invoke(_levelNumber);
        }
    }
}
