namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class PreComAlert
{
    public Guid Id { get; set; }
    public string? Alert { get; set; }
    public DateTime? SendTime { get; set; }
    public int? Priority { get; set; }
    public string? RawJson { get; set; }
}
