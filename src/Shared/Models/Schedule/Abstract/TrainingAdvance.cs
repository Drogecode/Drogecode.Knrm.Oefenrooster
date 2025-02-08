namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

public class TrainingAdvance : TrainingBase
{
    public Guid? TrainingId { get; set; }
    public Guid? DefaultId { get; set; }
    public Guid? PlannedFunctionId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool Updated { get; set; }
    public bool IsPinned { get; set; }
    public bool ShowTime { get; set; }
    public bool HasDescription { get; set; }
}
