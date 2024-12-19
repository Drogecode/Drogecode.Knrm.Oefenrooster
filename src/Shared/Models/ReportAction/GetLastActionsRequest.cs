namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

public class GetLastActionsRequest
{
    public List<Guid> Users { get; set; }
    public List<string>? Types { get; set; }
    public List<string>? Search { get; set; }
    public int Count { get; set; }
    public int Skip { get; set; }
}