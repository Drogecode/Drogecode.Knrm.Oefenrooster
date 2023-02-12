using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Helpers;

public class PlannerHelper
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
}
