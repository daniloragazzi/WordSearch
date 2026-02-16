using System.Collections.Generic;
using UnityEngine;

namespace RagazziStudios.Core.Infrastructure.Analytics
{
    /// <summary>
    /// Implementação mock de IAnalyticsService para desenvolvimento.
    /// Loga todos os eventos no Console.
    /// Será substituída por UnityAnalyticsService na integração (CFG-006).
    /// </summary>
    public class MockAnalyticsService : IAnalyticsService
    {
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            IsInitialized = true;
            Debug.Log("[Analytics/Mock] Initialized.");
        }

        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
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

            Debug.Log($"[Analytics/Mock] {eventName}{paramStr}");
        }

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
                { "session_duration", sessionDurationSeconds }
            });
        }
    }
}
