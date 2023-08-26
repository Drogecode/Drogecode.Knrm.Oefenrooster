using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Heron.MudCalendar;

namespace Drogecode.Knrm.Oefenrooster.Client.Models.CalendarItems;

public class RoosterItemDayCalendarItem : CalendarItem
{
    public RoosterItemDay? ItemDay { get; set; }
}
