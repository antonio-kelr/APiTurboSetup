using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace APiTurboSetup.Utils
{
    /// <summary>
    /// Classe utilitária para manipulação de slugs
    /// </summary>
    public static class SlugUtils
    {
        /// <summary>
        /// Verifica se um slug está em um formato válido.
        /// Um slug válido contém apenas letras minúsculas, números e hífens.
        /// </summary>
        /// <param name="slug">O slug a ser validado</param>
        /// <returns>Verdadeiro se o slug for válido, falso caso contrário</returns>
        public static bool IsValidSlug(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
                return false;
                
            // Permite apenas letras minúsculas, números e hífens
            return Regex.IsMatch(slug, @"^[a-z0-9\-]+$");
        }

        /// <summary>
        /// Gera um slug a partir de uma string.
        /// Remove acentos, converte para minúsculo e substitui espaços por hífens.
        /// </summary>
        /// <param name="text">Texto para converter em slug</param>
        /// <returns>O slug gerado</returns>
        public static string GenerateSlug(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            
            // Normaliza a string para remover acentos
            string normalizedString = text.Normalize(NormalizationForm.FormD);
            StringBuilder slug = new StringBuilder();
            
            // Percorre cada caractere da string normalizada
            foreach (char c in normalizedString)
            {
                // Pula caracteres de pontuação e acentos
                if (CharUnicodeInfo.GetUnicodeCategory(c) == UnicodeCategory.NonSpacingMark)
                    continue;
                
                slug.Append(c);
            }
            
            // Converte para minúsculo
            string result = slug.ToString().ToLowerInvariant();
            
            // Substitui espaços e caracteres não alfanuméricos por hífen
            result = Regex.Replace(result, @"[^a-z0-9\s-]", "");
            
            // Converte espaços em hífens
            result = Regex.Replace(result, @"\s+", "-");
            
            // Remove hífens duplicados
            result = Regex.Replace(result, @"-+", "-");
            
            // Remove hífen do início e do fim
            result = result.Trim('-');
            
            return result;
        }
    }
} 