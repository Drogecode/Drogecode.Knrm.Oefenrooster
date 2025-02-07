using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchAssignedUserResponse : BaseResponse
{
    public Guid? IdPatched { get; set; }
    public Guid? AvailableId { get; set; }
    public string? CalendarEventId { get; set; }
    public Availabilty? Availabilty { get; set; }
    public Availability? Availability { get; set; }
    public AvailabilitySetBy? SetBy { get; set; }
}
