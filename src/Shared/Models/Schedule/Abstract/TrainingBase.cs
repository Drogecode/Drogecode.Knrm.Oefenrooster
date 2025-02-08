namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;

public abstract class TrainingBase
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public virtual Guid? RoosterTrainingTypeId { get; set; }
}
