namespace Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;

public class ReportActionSharedConfiguration
{
    public Guid Id { get; set; }
    public List<Guid> SelectedUsers { get; set; } = [];
    public List<string>? Types { get; set; }
    public List<string>? Search { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ValidUntil { get; set; }
}