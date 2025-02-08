namespace Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;

public class GetTraininTypeByIdResponse : BaseResponse
{
    public PlannerTrainingType? TrainingType { get; set; }
}
