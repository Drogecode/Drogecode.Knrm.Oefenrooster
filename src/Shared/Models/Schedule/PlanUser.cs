using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlanUser
{
    public Guid UserId { get; set; }
    public Guid? TrainingId { get; set; }
    public Guid? UserFunctionId { get; set; }
    public Guid? PlannedFunctionId { get; set; }
    public Guid? VehicleId { get; set; }
    public Availabilty? Availabilty { get; set; }
    public AvailabilitySetBy SetBy { get; set; }
    public bool Assigned { get; set; }
    public bool ClickedFunction { get; set; }
    public string Name { get; set; } = "Some dude!";
    public string? CalendarEventId { get; set; }
}
