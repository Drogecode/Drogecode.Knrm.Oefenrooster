using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class OtherScheduleUserRequest
{
    public Guid? TrainingId { get; set; }
    public Guid? FunctionId { get; set; }
    public Guid? UserId { get; set; }
    public bool Assigned { get; set; }
    public TrainingAdvance? Training { get; set; }
}
