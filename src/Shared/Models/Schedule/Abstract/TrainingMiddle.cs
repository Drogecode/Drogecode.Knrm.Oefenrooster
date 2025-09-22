namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

public abstract class TrainingMiddle : TrainingBase
{
    public Guid? DefaultId { get; set; }
    public Guid? TrainingTargetSetId { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool IsPinned { get; set; }
    public bool IsPermanentPinned { get; set; }
    public bool ShowTime { get; set; }
    
}