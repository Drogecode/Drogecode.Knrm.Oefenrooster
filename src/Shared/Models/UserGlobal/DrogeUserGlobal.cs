namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

public class DrogeUserGlobal
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ExternalId { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatedBy { get; set; }
}