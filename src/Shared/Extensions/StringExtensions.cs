using System.Text.RegularExpressions;

namespace Drogecode.Knrm.Oefenrooster.Shared.Extensions;

public static partial class StringExtensions
{
    public static string TrimString(this string str, string trimString)
    {
        while (str.EndsWith(trimString))
        {
            str = str.Remove(str.Length - trimString.Length);
        }

        return str;
    }
    
    
    public static bool IsHtmlOnlyWhitespaceOrBreaks(this string html)
    {
        if (string.IsNullOrEmpty(html))
            return true;

        // Remove <br>, <br/>, <br /> (case-insensitive)
        var withoutBreaks = MyRegex().Replace(html, "");

        // Remove all HTML tags
        var withoutTags = MyRegex1().Replace(withoutBreaks, "");

        // Trim and check if it's now just whitespace
        return string.IsNullOrWhiteSpace(withoutTags);
    }

    [GeneratedRegex(@"<br\s*/?>", RegexOptions.IgnoreCase, "nl-NL")]
    private static partial Regex MyRegex();
    [GeneratedRegex(@"<[^>]+>")]
    private static partial Regex MyRegex1();
}