namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class LinkedCustomer
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public string? Name { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsCurrent { get; set; }
    public int Order { get; set; }
}