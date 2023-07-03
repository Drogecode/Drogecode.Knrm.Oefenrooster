using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class RoosterItemDayMapper
{
    public static DbRoosterItemDay ToDbRoosterItemDay(this RoosterItemDay roosterItemDay)
    {
        return new DbRoosterItemDay
        {
            Id = roosterItemDay.Id,
            DateStart = roosterItemDay.DateStart,
            DateEnd = roosterItemDay.DateEnd,
            IsFullDay = roosterItemDay.IsFullDay,
            Type = roosterItemDay.Type,
            Text = roosterItemDay.Text,
        };
    }
    public static RoosterItemDay ToRoosterItemDay(this DbRoosterItemDay roosterItemDay)
    {
        return new RoosterItemDay
        {
            Id = roosterItemDay.Id,
            DateStart = roosterItemDay.DateStart,
            DateEnd = roosterItemDay.DateEnd,
            IsFullDay = roosterItemDay.IsFullDay,
            Type = roosterItemDay.Type,
            Text = roosterItemDay.Text,
        };
    }
}
