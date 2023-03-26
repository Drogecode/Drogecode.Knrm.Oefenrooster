using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using MudBlazor;
using MudBlazor.Utilities;

namespace Drogecode.Knrm.Oefenrooster.Client.Helpers;

public static class PlannerHelper
{

    public static Color ColorAvailabilty(Availabilty? availabilty)
    {
        switch (availabilty)
        {
            case Availabilty.Available:
                return Color.Success;
            case Availabilty.NotAvailable:
                return Color.Error;
            case Availabilty.Maybe:
                return Color.Warning;
            case Availabilty.None:
            default:
                return Color.Inherit;
        }
    }

    public static string HeaderStyle(PlannerTrainingType? trainingType, bool isDark = false)
    {
        var color = isDark ? trainingType?.ColorDark : trainingType?.ColorLight;
        if (color == null)
            return $"background-color: {Color.Default}";
        else
            return $"background-color: {color}";
    }
}
