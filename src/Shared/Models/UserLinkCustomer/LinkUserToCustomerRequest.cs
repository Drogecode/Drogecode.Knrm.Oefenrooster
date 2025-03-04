namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class LinkUserToCustomerRequest
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public Guid? LinkedUserId { get; set; }
    public bool IsActive { get; set; }
    public bool CreateNew  { get; set; }
}