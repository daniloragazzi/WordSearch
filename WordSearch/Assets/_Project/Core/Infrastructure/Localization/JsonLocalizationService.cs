using System;
using System.Collections.Generic;
using UnityEngine;

namespace RagazziStudios.Core.Infrastructure.Localization
{
    /// <summary>
    /// Implementação de ILocalizationService usando arquivos JSON.
    /// Carrega strings de Resources/Data/localization/{languageCode}.json.
    /// </summary>
    public class JsonLocalizationService : ILocalizationService
    {
        private const string LOCALIZATION_PATH = "Data/localization/";
        private const string DEFAULT_LANGUAGE = "pt-BR";

        private Dictionary<string, string> _strings;

        public string CurrentLanguage { get; private set; }

        public JsonLocalizationService()
        {
            _strings = new Dictionary<string, string>();
            SetLanguage(DEFAULT_LANGUAGE);
        }

        public void SetLanguage(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode))
                languageCode = DEFAULT_LANGUAGE;

            var textAsset = Resources.Load<TextAsset>(LOCALIZATION_PATH + languageCode);

            if (textAsset == null)
            {
                Debug.LogWarning(
                    $"[Localization] Language file not found: {languageCode}. " +
                    $"Falling back to {DEFAULT_LANGUAGE}.");

                if (languageCode != DEFAULT_LANGUAGE)
                {
                    textAsset = Resources.Load<TextAsset>(LOCALIZATION_PATH + DEFAULT_LANGUAGE);
                }

                if (textAsset == null)
                {
                    Debug.LogError("[Localization] Default language file not found!");
                    CurrentLanguage = languageCode;
                    return;
                }

                languageCode = DEFAULT_LANGUAGE;
            }

            CurrentLanguage = languageCode;

            try
            {
                var data = JsonUtility.FromJson<LocalizationFile>(textAsset.text);
                _strings.Clear();

                if (data?.entries != null)
                {
                    foreach (var entry in data.entries)
                    {
                        _strings[entry.key] = entry.value;
                    }
                }

                Debug.Log($"[Localization] Loaded {_strings.Count} strings for '{languageCode}'.");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Localization] Error parsing language file: {e.Message}");
            }
        }

        public string Get(string key)
        {
            if (_strings.TryGetValue(key, out string value))
                return value;

            Debug.LogWarning($"[Localization] Missing key: '{key}'");
            return key;
        }

        public string Get(string key, params object[] args)
        {
            string template = Get(key);
            try
            {
                return string.Format(template, args);
            }
            catch (FormatException)
            {
                return template;
            }
        }

        public bool HasKey(string key) => _strings.ContainsKey(key);
    }

    /// <summary>Modelo do arquivo JSON de localização.</summary>
    [Serializable]
    public class LocalizationFile
    {
        public List<LocalizationEntry> entries;
    }

    /// <summary>Par chave-valor de localização.</summary>
    [Serializable]
    public class LocalizationEntry
    {
        public string key;
        public string value;
    }
}
