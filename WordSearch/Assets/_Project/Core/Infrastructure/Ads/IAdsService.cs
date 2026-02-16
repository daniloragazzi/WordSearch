using System;

namespace RagazziStudios.Core.Infrastructure.Ads
{
    /// <summary>
    /// Interface para serviço de anúncios.
    /// Abstrai o SDK de ads (AdMob, Unity Ads, etc.).
    /// </summary>
    public interface IAdsService
    {
        /// <summary>Se o serviço está inicializado e pronto.</summary>
        bool IsInitialized { get; }

        /// <summary>Inicializa o SDK de ads.</summary>
        void Initialize();

        // --- Interstitial ---

        /// <summary>Carrega um interstitial para exibição futura.</summary>
        void LoadInterstitial();

        /// <summary>Se há um interstitial pronto para exibir.</summary>
        bool IsInterstitialReady { get; }

        /// <summary>Exibe o interstitial carregado.</summary>
        void ShowInterstitial();

        /// <summary>Evento disparado quando o interstitial é fechado.</summary>
        event Action OnInterstitialClosed;

        // --- Rewarded ---

        /// <summary>Carrega um rewarded ad para exibição futura.</summary>
        void LoadRewarded();

        /// <summary>Se há um rewarded ad pronto para exibir.</summary>
        bool IsRewardedReady { get; }

        /// <summary>Exibe o rewarded ad.</summary>
        void ShowRewarded();

        /// <summary>Evento disparado quando o jogador completa o rewarded ad (ganhou recompensa).</summary>
        event Action OnRewardedCompleted;

        /// <summary>Evento disparado quando o rewarded ad é fechado sem completar.</summary>
        event Action OnRewardedFailed;
    }
}
