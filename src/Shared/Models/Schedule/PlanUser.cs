using System.Text.Json.Serialization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlanUser
{

    [JsonIgnore] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TrainingId { get; set; }
    public Guid? UserFunctionId { get; set; }
    public Guid? PlannedFunctionId { get; set; }
    public Guid? VehicleId { get; set; }
    public string? Buddy { get; set; }
    public Availability? Availability { get; set; }
    public AvailabilitySetBy SetBy { get; set; }
    public bool Assigned { get; set; }
    public bool ClickedFunction { get; set; }
    public string Name { get; set; } = "Some dude!";
    public string? CalendarEventId { get; set; }
}
