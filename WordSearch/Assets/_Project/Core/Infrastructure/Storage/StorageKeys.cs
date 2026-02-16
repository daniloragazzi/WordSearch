namespace RagazziStudios.Core.Infrastructure.Storage
{
    /// <summary>
    /// Helper para chaves de armazenamento do progresso.
    /// Centraliza a formatação das chaves para evitar erros.
    /// </summary>
    public static class StorageKeys
    {
        // Progresso de nível: "progress_{categoryId}_{levelNumber}" = 1 (completo)
        public static string LevelCompleted(string categoryId, int levelNumber) =>
            $"progress_{categoryId}_{levelNumber}";

        // Maior nível desbloqueado por categoria: "unlocked_{categoryId}" = levelNumber
        public static string HighestUnlocked(string categoryId) =>
            $"unlocked_{categoryId}";

        // Configurações
        public const string SOUND_ENABLED = "settings_sound";
        public const string MUSIC_ENABLED = "settings_music";
        public const string LANGUAGE = "settings_language";

        // Contadores para ads
        public const string LEVELS_SINCE_INTERSTITIAL = "ads_levels_count";

        // Sessão
        public const string FIRST_LAUNCH = "first_launch";
        public const string TOTAL_SESSIONS = "total_sessions";
    }
}
