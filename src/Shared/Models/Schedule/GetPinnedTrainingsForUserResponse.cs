namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

public class GetPinnedTrainingsForUserResponse : BaseResponse
{
    public List<Training> Trainings { get; set; } = new List<Training>();
}
