using System.Text.Json.Serialization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchAssignedUserResponse : BaseResponse
{
    public Guid? IdPatched { get; set; }
    public Guid? AvailableId { get; set; }
    public string? CalendarEventId { get; set; }
    [JsonIgnore] public Availability? Availability { get; set; }
    [JsonIgnore] public AvailabilitySetBy? SetBy { get; set; }
}