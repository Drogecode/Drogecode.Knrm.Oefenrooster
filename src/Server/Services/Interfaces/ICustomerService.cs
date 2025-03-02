using Drogecode.Knrm.Oefenrooster.Shared.Models.UserLinkCustomer;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerService
{
    Task<GetAllUserLinkCustomersResponse> GetAllLinkUserCustomers(Guid userId, Guid customerId);
    Task<PatchResponse> LinkUserToCustomer(Guid userId, Guid customerId, LinkUserToCustomerRequest body);
}