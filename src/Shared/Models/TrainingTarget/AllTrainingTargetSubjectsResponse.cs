namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTarget;

public class AllTrainingTargetSubjectsResponse : BaseMultipleResponse
{
    public List<TrainingSubject>? TrainingSubjects { get; set; }
}