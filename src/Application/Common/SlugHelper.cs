namespace Application.Common;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

   
        var normalizedText = text.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedText)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        
        var processedText = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        // Remove any invalid characters and replace spaces with hyphens
        processedText = Regex.Replace(processedText, @"[^a-z0-9\s-]", string.Empty);  // Remove invalid chars
        processedText = Regex.Replace(processedText, @"\s+", "-").Trim();            // Replace spaces with hyphens

        
        return Regex.Replace(processedText, @"-+", "-");                             // Collapse multiple hyphens
    }
}
