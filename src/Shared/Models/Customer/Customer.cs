namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "New customer";
    public string TimeZone { get; set; } = "Europe/Amsterdam";
    public DateTime Created { get; set; }
    public string? Instance { get; set; }
    public string? Domain { get; set; }
    public string? TenantId { get; set; }
}