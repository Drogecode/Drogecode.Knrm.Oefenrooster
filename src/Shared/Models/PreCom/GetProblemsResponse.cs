namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class GetProblemsResponse : BaseResponse
{
    public string? Problems { get; set; }
    public List<DateTime>? Dates { get; set; }
}