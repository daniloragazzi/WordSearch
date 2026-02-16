namespace RagazziStudios.Core.Infrastructure.Localization
{
    /// <summary>
    /// Interface para serviço de localização.
    /// Abstrai o carregamento de strings traduzidas.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>Idioma atualmente ativo (ex: "pt-BR").</summary>
        string CurrentLanguage { get; }

        /// <summary>Altera o idioma ativo.</summary>
        void SetLanguage(string languageCode);

        /// <summary>
        /// Retorna a string traduzida para a chave informada.
        /// Se a chave não existir, retorna a própria chave.
        /// </summary>
        string Get(string key);

        /// <summary>
        /// Retorna a string traduzida com formatação (string.Format).
        /// </summary>
        string Get(string key, params object[] args);

        /// <summary>
        /// Verifica se a chave existe no dicionário atual.
        /// </summary>
        bool HasKey(string key);
    }
}
