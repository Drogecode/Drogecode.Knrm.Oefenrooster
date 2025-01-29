namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

internal static class SettingNames
{
    internal static bool StringToBool(string shouldBeBool)
    {
        switch (shouldBeBool)
        {
            case "true":
            case "t":
            case "1":
            case "y":
            case "j":
                return true;
            default:
                return false;
        }
    }
}
