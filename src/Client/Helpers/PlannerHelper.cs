using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using MudBlazor;

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
    public static string HeaderClass(TrainingType trainingType)
    {
        switch (trainingType)
        {
            case TrainingType.EHBO:
                return "var(--mud-palette-warning-darken)";
            case TrainingType.OneOnOne:
                return "var(--mud-palette-tertiary-darken)";
            case TrainingType.FireBrigade:
                return "var(--mud-palette-error-darken)";
            case TrainingType.HRB:
                return "var(--mud-palette-success-lighten)";
            case TrainingType.Default:
            default:
                return "var(--mud-palette-lines-inputs)";
        }
    }
}
