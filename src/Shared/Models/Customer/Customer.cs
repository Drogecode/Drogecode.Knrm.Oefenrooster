namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "Europe/Amsterdam";
    public DateTime Created { get; set; }
    public string? Instance { get; set; }
    public string? Domain { get; set; }
    public string? TenantId { get; set; }
    public string? GroupId { get; set; }
}