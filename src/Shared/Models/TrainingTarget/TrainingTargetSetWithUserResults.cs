namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class TrainingTargetSetWithUserResults : TrainingTargetSet
{
    public List<TrainingTargetResult>? TrainingTargetResults { get; set; }
    public Guid RoosterAvailableId { get; set; }
}