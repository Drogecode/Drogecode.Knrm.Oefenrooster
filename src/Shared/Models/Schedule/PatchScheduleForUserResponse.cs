using System.Text.Json.Serialization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PatchScheduleForUserResponse : BaseResponse
{
    public Training? PatchedTraining { get; set; }
    public Guid? AvailableId { get; set; }
    public string? CalendarEventId { get; set; }
    [JsonIgnore] public Availability? Available { get; set; }
    [JsonIgnore] public AvailabilitySetBy? SetBy { get; set; }
}