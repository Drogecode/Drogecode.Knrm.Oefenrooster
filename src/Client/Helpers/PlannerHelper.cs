using System.Security.Claims;
using Drogecode.Knrm.Oefenrooster.Client.Models.Planner;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Helpers;

public static class PlannerHelper
{
    public static Color ColorAvailabilty(Availability? availabilty)
    {
        switch (availabilty)
        {
            case Availability.Available:
                return Color.Success;
            case Availability.NotAvailable:
                return Color.Error;
            case Availability.Maybe:
                return Color.Warning;
            case Availability.None:
            default:
                return Color.Inherit;
        }
    }

    public static string StyleAvailabilty(Availability? availabilty)
    {
        switch (availabilty)
        {
            case Availability.Available:
                return "bg-green-400";
            case Availability.NotAvailable:
                return "bg-red-400";
            case Availability.Maybe:
                return "bg-orange-400";
            case Availability.None:
            default:
                return "";
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

    public static DateTime? ForMonth(DateRange dateRange)
    {
        DateTime? forMonth = null;
        var findMonth = dateRange.Start!.Value;
        while (forMonth is null)
        {
            if (findMonth.Day != 1)
            {
                if (findMonth.Month == dateRange.End!.Value.Month)
                    break;
                findMonth = findMonth.AddDays(1);
                continue;
            }

            forMonth = findMonth;
        }

        return forMonth;
    }

    public static CanEditModel CanEdit(DateTime dateEnd, ClaimsPrincipal? user)
    {
        if (dateEnd >= DateTime.UtcNow.AddDays(AccessesSettings.AUTH_scheduler_edit_past_days))
        {
            return new CanEditModel { CanEdit = true, ShowPadlock = false };
        }

        if (user is not null)
        {
            var canEdit = user.IsInRole(AccessesNames.AUTH_scheduler_edit_past);
            return new CanEditModel { CanEdit = canEdit, ShowPadlock = true };
        }

        return new CanEditModel { CanEdit = false, ShowPadlock = false };
    }
}