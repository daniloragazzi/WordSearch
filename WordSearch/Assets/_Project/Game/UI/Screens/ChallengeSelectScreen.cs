using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RagazziStudios.Core.Application;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Tela de seleção de desafio.
    /// 3 botões para grids grandes (14x22, 18x22, 20x22) com 10 palavras mistas.
    /// </summary>
    public class ChallengeSelectScreen : MonoBehaviour
    {
        [Header("Botões de Desafio")]
        [SerializeField] private Button _challenge14x22;
        [SerializeField] private Button _challenge18x22;
        [SerializeField] private Button _challenge20x22;

        [Header("Navegação")]
        [SerializeField] private Button _backButton;

        [Header("Textos")]
        [SerializeField] private TMP_Text _titleText;

        private void OnEnable()
        {
            if (_challenge14x22 != null) _challenge14x22.onClick.AddListener(() => OnChallengeClicked(14, 22));
            if (_challenge18x22 != null) _challenge18x22.onClick.AddListener(() => OnChallengeClicked(18, 22));
            if (_challenge20x22 != null) _challenge20x22.onClick.AddListener(() => OnChallengeClicked(20, 22));
            if (_backButton != null) _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnDisable()
        {
            if (_challenge14x22 != null) _challenge14x22.onClick.RemoveAllListeners();
            if (_challenge18x22 != null) _challenge18x22.onClick.RemoveAllListeners();
            if (_challenge20x22 != null) _challenge20x22.onClick.RemoveAllListeners();
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
