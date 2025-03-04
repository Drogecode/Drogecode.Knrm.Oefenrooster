namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class LinkUserToCustomerResponse : PatchResponse
{
    public Guid? NewUserId { get; set; }
}