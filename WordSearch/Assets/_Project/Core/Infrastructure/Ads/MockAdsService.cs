using System;
using UnityEngine;

namespace RagazziStudios.Core.Infrastructure.Ads
{
    /// <summary>
    /// Implementação mock de IAdsService para desenvolvimento.
    /// Simula ads com logs no Console sem SDK real.
    /// Será substituída por AdMobService na integração (CFG-005).
    /// </summary>
    public class MockAdsService : IAdsService
    {
        public bool IsInitialized { get; private set; }
        public bool IsInterstitialReady { get; private set; }
        public bool IsRewardedReady { get; private set; }

        public event Action OnInterstitialClosed;
        public event Action OnRewardedCompleted;
        public event Action OnRewardedFailed;

        public void Initialize()
        {
            IsInitialized = true;
            Debug.Log("[Ads/Mock] Initialized.");
            LoadInterstitial();
            LoadRewarded();
        }

        // --- Interstitial ---

        public void LoadInterstitial()
        {
            IsInterstitialReady = true;
            Debug.Log("[Ads/Mock] Interstitial loaded (mock).");
        }

        public void ShowInterstitial()
        {
            if (!IsInterstitialReady)
            {
                Debug.LogWarning("[Ads/Mock] Interstitial not ready.");
                return;
            }

            Debug.Log("[Ads/Mock] === INTERSTITIAL SHOWN ===");
            IsInterstitialReady = false;
            OnInterstitialClosed?.Invoke();
            LoadInterstitial();
        }

        // --- Rewarded ---

        public void LoadRewarded()
        {
            IsRewardedReady = true;
            Debug.Log("[Ads/Mock] Rewarded loaded (mock).");
        }

        public void ShowRewarded()
        {
            if (!IsRewardedReady)
            {
                Debug.LogWarning("[Ads/Mock] Rewarded not ready.");
                OnRewardedFailed?.Invoke();
                return;
            }

            Debug.Log("[Ads/Mock] === REWARDED AD SHOWN === (auto-completing)");
            IsRewardedReady = false;
            OnRewardedCompleted?.Invoke();
            LoadRewarded();
        }
    }
}
