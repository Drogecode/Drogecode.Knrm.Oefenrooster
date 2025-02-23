namespace Drogecode.Knrm.Oefenrooster.Shared.Extensions;

public static class StringExtenstion
{
    public static string TrimString(this string str, string trimString)
    {
        while (str.EndsWith(trimString))
        {
            str = str.Remove(str.Length - trimString.Length);
        }

        return str;
    }
}