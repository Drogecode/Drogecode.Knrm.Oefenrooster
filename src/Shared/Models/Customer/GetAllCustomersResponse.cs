namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

public class GetAllCustomersResponse : BaseMultipleResponse
{
    public List<Models.Customer.Customer>? Customers { get; set; }
}