using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerService
{
    Task<GetAllUserLinkCustomersResponse> GetAllLinkUserCustomers(Guid userId, Guid customerId, CancellationToken clt);
    Task<GetAllCustomersResponse> GetAllCustomers(CancellationToken clt);
    Task<PutResponse> PutNewCustomer(Customer customer, CancellationToken clt);
    Task<LinkUserToCustomerResponse> LinkUserToCustomer(Guid userId, Guid customerId, LinkUserToCustomerRequest body, CancellationToken clt);
}