namespace Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

public class PreComForward
{
    public Guid Id { get; set; }
    public string? ForwardUrl { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
}