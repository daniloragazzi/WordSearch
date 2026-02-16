using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Tela de seleção de desafio.
    /// 3 botões para grids grandes (20x10, 20x14, 20x16) com 10 palavras mistas.
    /// </summary>
    public class ChallengeSelectScreen : MonoBehaviour
    {
        [Header("Botões de Desafio")]
        [SerializeField] private Button _challenge20x10;
        [SerializeField] private Button _challenge20x14;
        [SerializeField] private Button _challenge20x16;

        [Header("Navegação")]
        [SerializeField] private Button _backButton;

        [Header("Textos")]
        [SerializeField] private TMP_Text _titleText;

        private void OnEnable()
        {
            if (_challenge20x10 != null) _challenge20x10.onClick.AddListener(() => OnChallengeClicked(20, 10));
            if (_challenge20x14 != null) _challenge20x14.onClick.AddListener(() => OnChallengeClicked(20, 14));
            if (_challenge20x16 != null) _challenge20x16.onClick.AddListener(() => OnChallengeClicked(20, 16));
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDisable()
        {
            if (_challenge20x10 != null) _challenge20x10.onClick.RemoveAllListeners();
            if (_challenge20x14 != null) _challenge20x14.onClick.RemoveAllListeners();
            if (_challenge20x16 != null) _challenge20x16.onClick.RemoveAllListeners();
            if (_backButton != null) _backButton.onClick.RemoveAllListeners();
        }

        private void OnChallengeClicked(int rows, int cols)
        {
            if (GameManager.Instance == null) return;

            var levelManager = GameManager.Instance.LevelManager;
            levelManager.StartChallengeLevel(rows, cols, 10);

            GameManager.Instance.StateMachine.TransitionTo(GameStateType.Playing);
        }

        private void OnBackClicked()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.StateMachine.GoBack();
        }
    }
}
