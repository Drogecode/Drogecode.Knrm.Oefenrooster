namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

internal static class FilthyInputHelper
{
    internal static List<string>? CleanList(List<string>? input, ILogger logger)
    {
        List<string>? cleanedInput =  null;
        if (input is not null)
        {
            cleanedInput = [];
            foreach (var value in input.Select(value => value.Trim()))
            {
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
                    logger.LogWarning("Removed unclean value from body.Search `{filthy}`", value.Replace(Environment.NewLine, ""));
                }
                else
                {
                    cleanedInput.Add(value);
                }
            }

            if (cleanedInput!.Count == 0)
                cleanedInput = null;
        }

        return cleanedInput;
    }
}