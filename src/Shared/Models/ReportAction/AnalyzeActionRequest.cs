namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;

public class AnalyzeActionRequest
{
    public List<Guid?>? Users { get; set; }
    public IEnumerable<string?>? Prio { get; set; }
}