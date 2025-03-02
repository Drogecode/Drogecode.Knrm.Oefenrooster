namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

public class Customer
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsCurrent { get; set; }
    public int Order { get; set; }
}