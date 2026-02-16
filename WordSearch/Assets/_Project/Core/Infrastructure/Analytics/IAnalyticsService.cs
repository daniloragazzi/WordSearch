using System.Collections.Generic;

namespace RagazziStudios.Core.Infrastructure.Analytics
{
    /// <summary>
    /// Interface para serviço de analytics.
    /// Abstrai o provedor (Unity Analytics, Firebase, etc.).
    /// </summary>
    public interface IAnalyticsService
    {
        /// <summary>Se o serviço está inicializado.</summary>
        bool IsInitialized { get; }

        /// <summary>Inicializa o serviço de analytics.</summary>
        void Initialize();

        /// <summary>
        /// Registra um evento customizado.
        /// </summary>
        /// <param name="eventName">Nome do evento.</param>
        /// <param name="parameters">Parâmetros opcionais do evento.</param>
        void TrackEvent(string eventName, Dictionary<string, object> parameters = null);

        // --- Eventos pré-definidos (conforme DOC-007) ---

        /// <summary>App aberto.</summary>
        void TrackGameStart();

        /// <summary>Início de nível.</summary>
        void TrackLevelStart(string categoryId, int levelNumber, string difficulty);

        /// <summary>Nível concluído.</summary>
        void TrackLevelComplete(string categoryId, int levelNumber,
            float timeSeconds, int hintsUsed);

        /// <summary>Saiu do nível sem completar.</summary>
        void TrackLevelQuit(string categoryId, int levelNumber,
            float timeSeconds, int wordsFound);

        /// <summary>Usou dica (rewarded ad).</summary>
        void TrackHintUsed(string categoryId, int levelNumber);

        /// <summary>Ad exibido.</summary>
        void TrackAdShown(string adType);

        /// <summary>Ad clicado.</summary>
        void TrackAdClicked(string adType);

        /// <summary>Escolheu categoria.</summary>
        void TrackCategorySelected(string categoryId);

        /// <summary>Sessão encerrada.</summary>
        void TrackSessionEnd(float sessionDurationSeconds);
    }
}
