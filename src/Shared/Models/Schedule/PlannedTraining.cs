using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class PlannedTraining : TrainingAdvance
{
    public List<PlanUser> PlanUsers { get; set; } = new List<PlanUser>();
    public bool IsCreated { get; set; }
    public string? TrainingTypeName { get; set; }
    public bool HasDescription { get; set; }
}
