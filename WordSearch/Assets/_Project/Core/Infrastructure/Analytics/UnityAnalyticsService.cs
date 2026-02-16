using System.Collections.Generic;
using UnityEngine;

namespace RagazziStudios.Core.Infrastructure.Analytics
{
    /// <summary>
    /// Implementação Unity Analytics de IAnalyticsService.
    /// 
    /// SETUP NECESSÁRIO (CFG-006):
    /// 1. Unity Dashboard (https://dashboard.unity.com):
    ///    - Project Settings > Analytics > Enable Analytics
    ///    - Copiar Project ID
    /// 2. No Unity Editor:
    ///    - Edit > Project Settings > Services > Analytics > Enable
    ///    - Window > Package Manager > Unity Analytics (com.unity.services.analytics)
    /// 3. Consent/GDPR:
    ///    - Implementar fluxo de consentimento antes de Initialize()
    ///    - Usar AnalyticsService.Instance.StartDataCollection() após consentimento
    /// 4. Trocar _useMockServices = false no GameManager Inspector
    /// 
    /// Eventos definidos (DOC-007):
    /// - game_start: App aberto
    /// - level_start: Início do nível (category, level, difficulty)
    /// - level_complete: Nível concluído (category, level, time, hints)
    /// - level_quit: Saiu sem completar (category, level, time, words_found)
    /// - hint_used: Usou dica (category, level)
    /// - ad_shown: Ad exibido (type: interstitial/rewarded)
    /// - ad_clicked: Ad clicado (type)
    /// - category_selected: Escolheu categoria (category)
    /// - session_end: Sessão encerrada (duration)
    /// 
    /// Referência: https://docs.unity.com/ugs/en-us/manual/analytics/manual
    /// </summary>
    public class UnityAnalyticsService : IAnalyticsService
    {
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            // TODO: Descomentar quando Unity Analytics package estiver configurado
            //
            // try
            // {
            //     await UnityServices.InitializeAsync();
            //     AnalyticsService.Instance.StartDataCollection();
            //     IsInitialized = true;
            //     Debug.Log("[Analytics/Unity] Initialized.");
            // }
            // catch (Exception e)
            // {
            //     Debug.LogError($"[Analytics/Unity] Init failed: {e.Message}");
            //     IsInitialized = false;
            // }

            Debug.LogWarning("[Analytics/Unity] Package não configurado. Usando comportamento stub.");
            IsInitialized = true;
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!IsInitialized) return;

            // TODO: Descomentar quando Unity Analytics estiver configurado
            //
            // var customEvent = new CustomEvent(eventName);
            // if (parameters != null)
            // {
            //     foreach (var kvp in parameters)
            //     {
            //         customEvent.Add(kvp.Key, kvp.Value);
            //     }
            // }
            // AnalyticsService.Instance.RecordEvent(customEvent);

            // Log stub para desenvolvimento
            string paramStr = "";
            if (parameters != null && parameters.Count > 0)
            {
                var parts = new List<string>();
                foreach (var kvp in parameters)
                {
                    parts.Add($"{kvp.Key}={kvp.Value}");
                }
                paramStr = " | " + string.Join(", ", parts);
            }

            Debug.Log($"[Analytics/Unity] {eventName}{paramStr}");
        }

        // ═══════════════════════════════════════════════════
        //  Eventos Pré-definidos (DOC-007)
        // ═══════════════════════════════════════════════════

        public void TrackGameStart()
        {
            TrackEvent("game_start");
        }

        public void TrackLevelStart(string categoryId, int levelNumber, string difficulty)
        {
            TrackEvent("level_start", new Dictionary<string, object>
            {
                { "category", categoryId },
                { "level", levelNumber },
                { "difficulty", difficulty }
            });
        }

        public void TrackLevelComplete(string categoryId, int levelNumber,
            float timeSeconds, int hintsUsed)
        {
            TrackEvent("level_complete", new Dictionary<string, object>
            {
                { "category", categoryId },
                { "level", levelNumber },
                { "time_seconds", timeSeconds },
                { "hints_used", hintsUsed }
            });
        }

        public void TrackLevelQuit(string categoryId, int levelNumber,
            float timeSeconds, int wordsFound)
        {
            TrackEvent("level_quit", new Dictionary<string, object>
            {
                { "category", categoryId },
                { "level", levelNumber },
                { "time_seconds", timeSeconds },
                { "words_found", wordsFound }
            });
        }

        public void TrackHintUsed(string categoryId, int levelNumber)
        {
            TrackEvent("hint_used", new Dictionary<string, object>
            {
                { "category", categoryId },
                { "level", levelNumber }
            });
        }

        public void TrackAdShown(string adType)
        {
            TrackEvent("ad_shown", new Dictionary<string, object>
            {
                { "ad_type", adType }
            });
        }

        public void TrackAdClicked(string adType)
        {
            TrackEvent("ad_clicked", new Dictionary<string, object>
            {
                { "ad_type", adType }
            });
        }

        public void TrackCategorySelected(string categoryId)
        {
            TrackEvent("category_selected", new Dictionary<string, object>
            {
                { "category", categoryId }
            });
        }

        public void TrackSessionEnd(float sessionDurationSeconds)
        {
            TrackEvent("session_end", new Dictionary<string, object>
            {
                { "duration_seconds", sessionDurationSeconds }
            });
        }
    }
}
