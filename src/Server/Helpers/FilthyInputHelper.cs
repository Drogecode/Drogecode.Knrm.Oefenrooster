namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

internal static class FilthyInputHelper
{
    internal static List<string>? CleanList(List<string>? input, int maxLength, ILogger logger)
    {
        if (input is null) return null;
        List<string>? cleanedInput = [];
        var count = 0;
        foreach (var value in input.Select(value => value.Trim().Replace(Environment.NewLine, "")))
        {
            count++;
            if (count > maxLength)
            {
                logger.LogWarning("Exceeded maximum length of filthy input. {count} > {maxlength}", input.Count, maxLength);
                break;
            }

            if (string.IsNullOrEmpty(value) || value.Any(ch =>
                {
                    var isWrong = !char.IsLetterOrDigit(ch);
                    if (isWrong)
                    {
                        // Allow white space
                        isWrong = !char.IsWhiteSpace(ch);
                    }

                    return isWrong;
                }))
            {
                logger.LogWarning("Removed unclean value from body.Search `{filthy}`", value);
            }
            else
            {
                cleanedInput.Add(value);
            }
        }

        if (cleanedInput.Count == 0)
            cleanedInput = null;

        return cleanedInput;
    }
}