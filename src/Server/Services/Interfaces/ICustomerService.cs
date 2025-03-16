using Drogecode.Knrm.Oefenrooster.Shared.Models.Customer;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerService
{
    Task<GetAllCustomersResponse> GetAllCustomers(int take, int skip, CancellationToken clt);
    Task<GetCustomerResponse> GetCustomerById(GetCustomerRequest body, CancellationToken clt);
    Task<PutResponse> PutNewCustomer(Customer customer, CancellationToken clt);
    Task<PatchResponse> PatchCustomer(Customer customer, CancellationToken clt);
}