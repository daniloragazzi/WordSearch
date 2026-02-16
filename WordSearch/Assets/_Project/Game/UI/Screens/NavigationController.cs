using UnityEngine;
using UnityEngine.UI;

namespace RagazziStudios.Game.UI.Screens
{
    /// <summary>
    /// Controla a navegação entre telas na cena MainMenu.
    /// Escuta eventos do GameStateMachine para ativar/desativar telas.
    /// </summary>
    public class NavigationController : MonoBehaviour
    {
        [Header("Telas")]
        [SerializeField] private GameObject _mainMenuScreen;
        [SerializeField] private GameObject _categorySelectScreen;
        [SerializeField] private GameObject _levelSelectScreen;

        private Core.Application.GameStateMachine _stateMachine;

        private void Start()
        {
            if (Core.Application.GameManager.Instance == null)
            {
                Debug.LogError("[NavigationController] GameManager not found!");
                return;
            }

            _stateMachine = Core.Application.GameManager.Instance.StateMachine;
            _stateMachine.OnStateChanged += OnStateChanged;

            // Aplicar estado inicial
            ApplyState(_stateMachine.CurrentState);
        }

        private void OnDestroy()
        {
            if (_stateMachine != null)
                _stateMachine.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(Core.Application.GameStateType from,
            Core.Application.GameStateType to)
        {
            ApplyState(to);
        }

        private void ApplyState(Core.Application.GameStateType state)
        {
            _mainMenuScreen?.SetActive(state == Core.Application.GameStateType.MainMenu);
            _categorySelectScreen?.SetActive(state == Core.Application.GameStateType.CategorySelect);
            _levelSelectScreen?.SetActive(state == Core.Application.GameStateType.LevelSelect);

            // Se entrou em Playing, carregar cena Game
            if (state == Core.Application.GameStateType.Playing)
            {
                Core.Application.GameManager.Instance.LoadScene("Game");
            }
        }
    }
}
