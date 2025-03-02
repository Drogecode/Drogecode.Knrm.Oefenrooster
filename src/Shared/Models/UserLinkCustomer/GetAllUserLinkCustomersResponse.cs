
namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class GetAllUserLinkCustomersResponse : BaseMultipleResponse
{
    public List<Customer.Customer>? UserLinkedCustomers { get; set; }
}