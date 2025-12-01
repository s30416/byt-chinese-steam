using System.Text;
using System.Text.RegularExpressions;

namespace BytChineseSteam.Models.Util;

// contains methods to create game slugs using game titles
// written using ChatGPT, prompt: "write a c# function converting the title of the game to it's game slug"
// screenshot saved
public static class Slugifier
{
    public static string ToGameSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return string.Empty;

        // lowercase
        var slug = title.ToLowerInvariant();

        // replace diacritics (ł -> l, etc...)
        slug = RemoveDiacritics(slug);

        // replace all symbols, which are not numbers or letters, with hyphens
        // for example ":" is replaced with a -
        slug = Regex.Replace(slug, @"[^a-z0-9]+", "-");

        // trim hyphens from start/end
        slug = slug.Trim('-');

        return slug;
    }

    private static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder();

        foreach (var c in normalized)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }

        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}