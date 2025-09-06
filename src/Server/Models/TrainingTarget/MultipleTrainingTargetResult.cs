using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

namespace Drogecode.Knrm.Oefenrooster.Server.Models.TrainingTarget;

public class MultipleTrainingTargetResult
{
    public List<TrainingTargetResult>? TargetResults { get; set; }
    public List<Guid>? TrainingTargetIds { get; set; }
}