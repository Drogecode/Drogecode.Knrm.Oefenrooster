using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class RoosterItemMonthMapper
{
    public static DbRoosterItemMonth ToDbRoosterItemMonth(this RoosterItemMonth roosterItemMonth)
    {
        return new DbRoosterItemMonth
        {
            Id = roosterItemMonth.Id,
            Month = roosterItemMonth.Month,
            Year = roosterItemMonth.Year,
            Type = roosterItemMonth.Type,
            Severity = roosterItemMonth.Severity,
            Text = roosterItemMonth.Text,
            Order = roosterItemMonth.Order,
        };
    }
    public static RoosterItemMonth ToRoosterItemMonth(this DbRoosterItemMonth roosterItemMonth)
    {
        return new RoosterItemMonth
        {
            Id = roosterItemMonth.Id,
            Month = roosterItemMonth.Month,
            Year = roosterItemMonth.Year,
            Type = roosterItemMonth.Type,
            Severity = roosterItemMonth.Severity,
            Text = roosterItemMonth.Text,
            Order = roosterItemMonth.Order,
        };
    }
}
