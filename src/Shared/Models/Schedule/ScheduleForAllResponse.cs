namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class ScheduleForAllResponse : BaseResponse
{
    public List<PlannedTraining> Planners { get; set; } = new List<PlannedTraining>();
    public List<UserTrainingCounter> UserTrainingCounters { get; set; } = new List<UserTrainingCounter>();
}
