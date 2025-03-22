using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserLinkCustomerService
{
    Task<GetAllUsersWithLinkToCustomerResponse> GetAllUsersWithLinkToCustomer(Guid currentCustomerId, Guid linkedCustomerId, CancellationToken clt);
    Task<GetAllUserLinkCustomersResponse> GetAllLinkUserCustomers(Guid userId, Guid customerId, CancellationToken clt);
    Task<LinkUserToCustomerResponse> LinkUserToCustomer(Guid userId, Guid customerId, LinkUserToCustomerRequest body, CancellationToken clt);
}