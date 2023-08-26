using Heron.MudCalendar;

namespace Drogecode.Knrm.Oefenrooster.Client.Models.CalendarItems;


public class ScheduleCalendarItem : CalendarItem
{
    public PlannedTraining? Training { get; set; }
}
