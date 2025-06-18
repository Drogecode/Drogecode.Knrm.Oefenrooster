using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerService
{
    Task<GetAllCustomersResponse> GetAllCustomers(int take, int skip, CancellationToken clt);
    Task<GetCustomerResponse> GetCustomerById(Guid customerId, CancellationToken clt);
    Task<List<CustomerAuthentication>> GetByTenantId(string externalCustomerId, CancellationToken clt);
    Task<PutResponse> PutNewCustomer(Customer customer, CancellationToken clt);
    Task<PatchResponse> PatchCustomer(Customer customer, CancellationToken clt);
}