using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using RagazziStudios.Core.Infrastructure;
using RagazziStudios.Core.Infrastructure.Ads;
using RagazziStudios.Core.Infrastructure.Analytics;
using RagazziStudios.Core.Infrastructure.Localization;
using RagazziStudios.Core.Infrastructure.Storage;

namespace RagazziStudios.Core.Application
{
    /// <summary>
    /// Orquestrador principal do jogo. Singleton persistente entre cenas.
    /// Inicializa serviços, gerencia state machine e coordena fluxo.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance => _instance;

        [Header("Debug")]
        [SerializeField] private bool _useMockServices = true;

        /// <summary>State machine do game flow.</summary>
        public GameStateMachine StateMachine { get; private set; }

        /// <summary>Gerenciador de progressão de níveis.</summary>
        public LevelManager LevelManager { get; private set; }

        /// <summary>Hora de início da sessão.</summary>
        private float _sessionStartTime;

        // --- Eventos globais ---

        /// <summary>Evento: dica usada (para UI + analytics).</summary>
        public event Action OnHintUsed;

        /// <summary>Evento: serviços inicializados, pronto para jogar.</summary>
        public event Action OnServicesReady;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeServices();
            InitializeManagers();

            _sessionStartTime = Time.realtimeSinceStartup;
        }

        private void InitializeServices()
        {
            // Storage — sempre PlayerPrefs
            var storage = new PlayerPrefsStorage();
            ServiceLocator.Register<IStorageService>(storage);

            // Localization
            var localization = new JsonLocalizationService();
            string savedLang = storage.GetString(StorageKeys.LANGUAGE, "pt-BR");
            localization.SetLanguage(savedLang);
            ServiceLocator.Register<ILocalizationService>(localization);

            // Ads — mock para dev, AdMob para produção
            if (_useMockServices)
            {
                var ads = new MockAdsService();
                ads.Initialize();
                ServiceLocator.Register<IAdsService>(ads);
            }
            else
            {
                var ads = new AdMobService();
                ads.Initialize();
                ServiceLocator.Register<IAdsService>(ads);
            }

            // Analytics — mock para dev, Unity Analytics para produção
            if (_useMockServices)
            {
                var analytics = new MockAnalyticsService();
                analytics.Initialize();
                ServiceLocator.Register<IAnalyticsService>(analytics);
            }
            else
            {
                var analytics = new UnityAnalyticsService();
                analytics.Initialize();
                ServiceLocator.Register<IAnalyticsService>(analytics);
            }

            // Incrementar sessões
            int sessions = storage.GetInt(StorageKeys.TOTAL_SESSIONS, 0);
            storage.SetInt(StorageKeys.TOTAL_SESSIONS, sessions + 1);

            // First launch
            if (!storage.HasKey(StorageKeys.FIRST_LAUNCH))
            {
                storage.SetBool(StorageKeys.FIRST_LAUNCH, true);
            }

            storage.Save();

            Debug.Log("[GameManager] Services initialized.");
        }

        private void InitializeManagers()
        {
            StateMachine = new GameStateMachine();
            LevelManager = new LevelManager();

            Debug.Log("[GameManager] Managers initialized.");
        }

        private void Start()
        {
            // Track game start
            if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            {
                analytics.TrackGameStart();
            }

            OnServicesReady?.Invoke();

            // Transicionar para MainMenu
            StateMachine.TransitionTo(GameStateType.MainMenu);
        }

        private void OnApplicationQuit()
        {
            // Track session end
            float sessionDuration = Time.realtimeSinceStartup - _sessionStartTime;

            if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
            {
                analytics.TrackSessionEnd(sessionDuration);
            }

            // Salvar
            if (ServiceLocator.TryGet<IStorageService>(out var storage))
            {
                storage.Save();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                // App minimizado — salvar
                if (ServiceLocator.TryGet<IStorageService>(out var storage))
                {
                    storage.Save();
                }
            }
        }

        // --- Ações públicas ---

        /// <summary>
        /// Carrega uma cena pelo nome.
        /// </summary>
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Solicita uma dica (rewarded ad).
        /// </summary>
        public void RequestHint()
        {
            if (!ServiceLocator.TryGet<IAdsService>(out var ads))
                return;

            if (!ads.IsRewardedReady)
            {
                Debug.LogWarning("[GameManager] Rewarded ad not ready.");
                return;
            }

            // Inscrever no callback antes de mostrar
            void OnRewarded()
            {
                ads.OnRewardedCompleted -= OnRewarded;
                OnHintUsed?.Invoke();

                if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
                {
                    analytics.TrackHintUsed(
                        LevelManager.CurrentCategoryId,
                        LevelManager.CurrentLevelNumber);
                    analytics.TrackAdShown("rewarded");
                }
            }

            ads.OnRewardedCompleted += OnRewarded;
            ads.ShowRewarded();
        }

        /// <summary>
        /// Verifica e mostra interstitial (a cada 3 níveis completos).
        /// </summary>
        public void TryShowInterstitial()
        {
            if (!ServiceLocator.TryGet<IStorageService>(out var storage))
                return;

            if (!ServiceLocator.TryGet<IAdsService>(out var ads))
                return;

            int count = storage.GetInt(StorageKeys.LEVELS_SINCE_INTERSTITIAL, 0);
            count++;

            if (count >= 3 && ads.IsInterstitialReady)
            {
                ads.ShowInterstitial();
                storage.SetInt(StorageKeys.LEVELS_SINCE_INTERSTITIAL, 0);

                if (ServiceLocator.TryGet<IAnalyticsService>(out var analytics))
                {
                    analytics.TrackAdShown("interstitial");
                }
            }
            else
            {
                storage.SetInt(StorageKeys.LEVELS_SINCE_INTERSTITIAL, count);
            }

            storage.Save();
        }
    }
}
