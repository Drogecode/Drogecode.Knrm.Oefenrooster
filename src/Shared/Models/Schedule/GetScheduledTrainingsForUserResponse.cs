namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class GetScheduledTrainingsForUserResponse : BaseMultipleResponse
{
    public List<PlannedTraining> Trainings { get; set; } = new List<PlannedTraining>();
}
