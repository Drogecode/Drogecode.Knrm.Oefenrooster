namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

internal static class SettingNames
{
    internal const string TRAINING_TO_CALENDAR = "TrainingToCalendar";
    internal const string IOS_DARK_LIGHT_CHECK = "IosDarkLightCheck";

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
