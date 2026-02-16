using System;
using UnityEngine;

namespace RagazziStudios.Core.Infrastructure.Ads
{
    /// <summary>
    /// Implementação AdMob de IAdsService.
    /// 
    /// SETUP NECESSÁRIO (CFG-005):
    /// 1. Importar Google Mobile Ads Unity Plugin v9.x
    ///    → Window > Package Manager > + > Add from git URL:
    ///    → https://github.com/googleads/googleads-mobile-unity/releases
    /// 2. Assets > Google Mobile Ads > Settings:
    ///    - Android App ID: ca-app-pub-XXXXXXXXXXXXXXXX~YYYYYYYYYY
    ///    - Delay app measurement: ON
    /// 3. Trocar _useMockServices = false no GameManager Inspector
    /// 4. Para produção, substituir Test IDs pelos reais do AdMob Console
    /// 
    /// Test Ad Unit IDs (Android):
    /// - Interstitial: ca-app-pub-3940256099942544/1033173712
    /// - Rewarded:     ca-app-pub-3940256099942544/5224354917
    /// 
    /// Referência: https://developers.google.com/admob/unity/quick-start
    /// </summary>
    public class AdMobService : IAdsService
    {
        // ─── Test Ad Unit IDs (Android) ───
        // IMPORTANTE: Substituir por IDs reais antes de publicar!
        private const string INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
        private const string REWARDED_ID = "ca-app-pub-3940256099942544/5224354917";

        public bool IsInitialized { get; private set; }
        public bool IsInterstitialReady { get; private set; }
        public bool IsRewardedReady { get; private set; }

        public event Action OnInterstitialClosed;
        public event Action OnRewardedCompleted;
        public event Action OnRewardedFailed;

        public void Initialize()
        {
            // TODO: Descomentar quando Google Mobile Ads SDK estiver importado
            //
            // MobileAds.Initialize(status =>
            // {
            //     Debug.Log("[Ads/AdMob] SDK initialized.");
            //     IsInitialized = true;
            //     LoadInterstitial();
            //     LoadRewarded();
            // });

            // Fallback enquanto SDK não está disponível
            Debug.LogWarning("[Ads/AdMob] SDK não importado. Usando comportamento stub.");
            IsInitialized = true;
            LoadInterstitial();
            LoadRewarded();
        }

        // ═══════════════════════════════════════════════════
        //  Interstitial
        // ═══════════════════════════════════════════════════

        public void LoadInterstitial()
        {
            if (!IsInitialized) return;

            // TODO: Descomentar quando SDK estiver importado
            //
            // var request = new AdRequest();
            // InterstitialAd.Load(INTERSTITIAL_ID, request, (ad, error) =>
            // {
            //     if (error != null)
            //     {
            //         Debug.LogError($"[Ads/AdMob] Interstitial load failed: {error}");
            //         IsInterstitialReady = false;
            //         return;
            //     }
            //     
            //     _interstitialAd = ad;
            //     IsInterstitialReady = true;
            //     
            //     ad.OnAdFullScreenContentClosed += () =>
            //     {
            //         IsInterstitialReady = false;
            //         OnInterstitialClosed?.Invoke();
            //         LoadInterstitial(); // Pre-load next
            //     };
            //     
            //     Debug.Log("[Ads/AdMob] Interstitial loaded.");
            // });

            IsInterstitialReady = true;
            Debug.Log("[Ads/AdMob] Interstitial loaded (stub).");
        }

        public void ShowInterstitial()
        {
            if (!IsInterstitialReady)
            {
                Debug.LogWarning("[Ads/AdMob] Interstitial not ready.");
                return;
            }

            // TODO: Descomentar quando SDK estiver importado
            // _interstitialAd?.Show();

            Debug.Log("[Ads/AdMob] === INTERSTITIAL SHOWN (stub) ===");
            IsInterstitialReady = false;
            OnInterstitialClosed?.Invoke();
            LoadInterstitial();
        }

        // ═══════════════════════════════════════════════════
        //  Rewarded
        // ═══════════════════════════════════════════════════

        public void LoadRewarded()
        {
            if (!IsInitialized) return;

            // TODO: Descomentar quando SDK estiver importado
            //
            // var request = new AdRequest();
            // RewardedAd.Load(REWARDED_ID, request, (ad, error) =>
            // {
            //     if (error != null)
            //     {
            //         Debug.LogError($"[Ads/AdMob] Rewarded load failed: {error}");
            //         IsRewardedReady = false;
            //         return;
            //     }
            //     
            //     _rewardedAd = ad;
            //     IsRewardedReady = true;
            //     
            //     ad.OnAdFullScreenContentClosed += () =>
            //     {
            //         IsRewardedReady = false;
            //         LoadRewarded(); // Pre-load next
            //     };
            //     
            //     Debug.Log("[Ads/AdMob] Rewarded loaded.");
            // });

            IsRewardedReady = true;
            Debug.Log("[Ads/AdMob] Rewarded loaded (stub).");
        }

        public void ShowRewarded()
        {
            if (!IsRewardedReady)
            {
                Debug.LogWarning("[Ads/AdMob] Rewarded not ready.");
                OnRewardedFailed?.Invoke();
                return;
            }

            // TODO: Descomentar quando SDK estiver importado
            //
            // _rewardedAd?.Show(reward =>
            // {
            //     Debug.Log($"[Ads/AdMob] Reward granted: {reward.Amount} {reward.Type}");
            //     OnRewardedCompleted?.Invoke();
            // });

            Debug.Log("[Ads/AdMob] === REWARDED SHOWN (stub) ===");
            IsRewardedReady = false;
            OnRewardedCompleted?.Invoke();
            LoadRewarded();
        }
    }
}
