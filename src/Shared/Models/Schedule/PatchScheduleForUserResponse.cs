using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchScheduleForUserResponse : BaseResponse
{
    public Training? PatchedTraining { get; set; }
    public Guid? AvailableId { get; set; }
    public string? CalendarEventId { get; set; }
}
