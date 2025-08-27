namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class GetAllTargetSetResponse : BaseMultipleResponse
{
    public List<TrainingTargetSet>? TrainingTargetSets { get; set; }
}