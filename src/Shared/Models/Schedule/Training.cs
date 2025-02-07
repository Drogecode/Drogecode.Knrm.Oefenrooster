using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class Training : TrainingAdvance
{
    public Availabilty? Availabilty { get; set; }//ToDo Remove when all users on v0.3.50 or above
    public Availability? Availability { get; set; }
    public AvailabilitySetBy SetBy { get; set; }
    public bool Assigned { get; set; }
}
