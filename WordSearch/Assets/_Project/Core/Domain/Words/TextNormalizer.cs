using System.Globalization;
using System.Text;

namespace RagazziStudios.Core.Domain.Words
{
    /// <summary>
    /// Utilitário para normalização de texto.
    /// Remove acentos e converte para maiúsculas.
    /// Classe pura C# — sem dependência de Unity.
    /// </summary>
    public static class TextNormalizer
    {
        /// <summary>
        /// Normaliza uma palavra para uso no grid:
        /// - Remove acentos (CORAÇÃO → CORACAO)
        /// - Converte para maiúsculas
        /// - Remove espaços e hífens
        /// </summary>
        public static string Normalize(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            // Normalizar para decompor caracteres acentuados
            string normalized = word.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder(normalized.Length);
            foreach (char c in normalized)
            {
                var category = CharUnicodeInfo.GetUnicodeCategory(c);
                // Ignorar marcas de acento (NonSpacingMark)
                if (category != UnicodeCategory.NonSpacingMark &&
                    c != ' ' && c != '-' && c != '\'')
                {
                    sb.Append(c);
                }
            }

            return sb.ToString().Normalize(NormalizationForm.FormC).ToUpperInvariant();
        }

        /// <summary>
        /// Formata para exibição na lista de palavras:
        /// - Maiúsculas
        /// - Mantém acentos
        /// </summary>
        public static string ToDisplay(string word)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            return word.ToUpperInvariant();
        }
    }
}
