using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PutAssignedUserResponse : BaseResponse
{
    public Guid? IdPut { get; set; }
    public Guid? AvailableId { get; set; }
    public string? CalendarEventId { get; set; }
    public Availabilty? Availabilty { get; set; }
    public Availability? Availability { get; set; }
    public AvailabilitySetBy? SetBy { get; set; }
}
