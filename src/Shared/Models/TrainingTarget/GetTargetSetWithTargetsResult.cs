namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class GetTargetSetWithTargetsResult : BaseMultipleResponse
{
    public Guid? RoosterAvailableId { get; set; }
    public TrainingTargetSet? TrainingTargetSet { get; set; }
    public List<TrainingTarget>? TrainingTargets { get; set; }
    public List<TrainingTargetResult>? TrainingTargetResults { get; set; }
}