namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

public class TrainingAdvance : TrainingMiddle
{
    public Guid? TrainingId { get; set; }
    public Guid? PlannedFunctionId { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public bool Updated { get; set; }
    public bool HasDescription { get; set; }
}
