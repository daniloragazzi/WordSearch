using System;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Estados do game flow.
    /// </summary>
    public enum GameStateType
    {
        Boot,
        MainMenu,
        CategorySelect,
        LevelSelect,
        Playing,
        Win,
        Pause
    }

    /// <summary>
    /// State machine do fluxo do jogo.
    /// Gerencia transições entre estados e dispara eventos.
    /// </summary>
    public class GameStateMachine
    {
        /// <summary>Estado atual.</summary>
        public GameStateType CurrentState { get; private set; }

        /// <summary>Estado anterior (para voltar).</summary>
        public GameStateType PreviousState { get; private set; }

        /// <summary>Evento disparado ao mudar de estado. Args: (estadoAnterior, estadoNovo).</summary>
        public event Action<GameStateType, GameStateType> OnStateChanged;

        public GameStateMachine()
        {
            CurrentState = GameStateType.Boot;
            PreviousState = GameStateType.Boot;
        }

        /// <summary>
        /// Transiciona para um novo estado.
        /// Valida a transição antes de aplicar.
        /// </summary>
        public bool TransitionTo(GameStateType newState)
        {
            if (newState == CurrentState)
                return false;

            if (!IsValidTransition(CurrentState, newState))
                return false;

            PreviousState = CurrentState;
            CurrentState = newState;
            OnStateChanged?.Invoke(PreviousState, CurrentState);

            return true;
        }

        /// <summary>
        /// Volta para o estado anterior (ex: fechar popup).
        /// </summary>
        public bool GoBack()
        {
            return TransitionTo(GetBackState(CurrentState));
        }

        /// <summary>
        /// Verifica se a transição entre dois estados é válida.
        /// </summary>
        private bool IsValidTransition(GameStateType from, GameStateType to)
        {
            switch (from)
            {
                case GameStateType.Boot:
                    return to == GameStateType.MainMenu;

                case GameStateType.MainMenu:
                    return to == GameStateType.CategorySelect;

                case GameStateType.CategorySelect:
                    return to == GameStateType.LevelSelect
                        || to == GameStateType.MainMenu;

                case GameStateType.LevelSelect:
                    return to == GameStateType.Playing
                        || to == GameStateType.CategorySelect;

                case GameStateType.Playing:
                    return to == GameStateType.Win
                        || to == GameStateType.Pause
                        || to == GameStateType.LevelSelect;

                case GameStateType.Win:
                    return to == GameStateType.Playing
                        || to == GameStateType.LevelSelect;

                case GameStateType.Pause:
                    return to == GameStateType.Playing
                        || to == GameStateType.LevelSelect
                        || to == GameStateType.MainMenu;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Retorna o estado "voltar" para cada estado atual.
        /// </summary>
        private GameStateType GetBackState(GameStateType current)
        {
            switch (current)
            {
                case GameStateType.CategorySelect:
                    return GameStateType.MainMenu;
                case GameStateType.LevelSelect:
                    return GameStateType.CategorySelect;
                case GameStateType.Playing:
                    return GameStateType.LevelSelect;
                case GameStateType.Pause:
                    return GameStateType.Playing;
                case GameStateType.Win:
                    return GameStateType.LevelSelect;
                default:
                    return GameStateType.MainMenu;
            }
        }
    }
}
