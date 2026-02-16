namespace RagazziStudios.Core.Infrastructure.Storage
{
    /// <summary>
    /// Interface para persistência de dados do jogador.
    /// Abstrai o mecanismo de armazenamento (PlayerPrefs, arquivo, cloud, etc.).
    /// </summary>
    public interface IStorageService
    {
        /// <summary>Salva um valor inteiro.</summary>
        void SetInt(string key, int value);

        /// <summary>Retorna um valor inteiro, ou defaultValue se não existir.</summary>
        int GetInt(string key, int defaultValue = 0);

        /// <summary>Salva um valor string.</summary>
        void SetString(string key, string value);

        /// <summary>Retorna um valor string, ou defaultValue se não existir.</summary>
        string GetString(string key, string defaultValue = "");

        /// <summary>Salva um valor float.</summary>
        void SetFloat(string key, float value);

        /// <summary>Retorna um valor float, ou defaultValue se não existir.</summary>
        float GetFloat(string key, float defaultValue = 0f);

        /// <summary>Salva um valor booleano.</summary>
        void SetBool(string key, bool value);

        /// <summary>Retorna um valor booleano, ou defaultValue se não existir.</summary>
        bool GetBool(string key, bool defaultValue = false);

        /// <summary>Verifica se a chave existe.</summary>
        bool HasKey(string key);

        /// <summary>Remove uma chave.</summary>
        void DeleteKey(string key);

        /// <summary>Remove todos os dados salvos.</summary>
        void DeleteAll();

        /// <summary>Força gravação em disco (para implementações com buffer).</summary>
        void Save();
    }
}
