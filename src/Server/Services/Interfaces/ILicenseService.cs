using Drogecode.Knrm.Oefenrooster.Shared.Models.License;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ILicenseService
{
    Task<GetAllLicensesResponse> GetAllLicensesForCustomer(Guid customerId, CancellationToken clt);
}