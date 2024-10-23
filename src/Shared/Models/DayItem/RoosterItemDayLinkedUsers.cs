using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

public sealed class RoosterItemDayLinkedUsers
{
    public Guid UserId { get; set; }
    public string? ExternalId { get; set; }
    public string? CalendarEventId { get; set; }
}
