using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class Training : TrainingAdvance
{
    public Availability? Availability { get; set; }
    public AvailabilitySetBy SetBy { get; set; }
    public bool Assigned { get; set; }
}
