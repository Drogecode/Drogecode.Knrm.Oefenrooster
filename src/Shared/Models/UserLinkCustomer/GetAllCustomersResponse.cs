namespace Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

public class GetAllCustomersResponse : BaseMultipleResponse
{
    public List<Customer.Customer>? Customers { get; set; }
}