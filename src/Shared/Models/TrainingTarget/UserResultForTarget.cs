namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class UserResultForTarget
{
    public Guid TrainingTargetId { get; set; }
    public double Result { get; set; }
    public int Count { get; set; }
}