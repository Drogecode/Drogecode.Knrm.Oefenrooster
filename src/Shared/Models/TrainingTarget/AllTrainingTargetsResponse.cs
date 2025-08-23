namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class AllTrainingTargetsResponse : BaseMultipleResponse
{
    public List<TrainingSubject>? TrainingSubjects { get; set; }
}