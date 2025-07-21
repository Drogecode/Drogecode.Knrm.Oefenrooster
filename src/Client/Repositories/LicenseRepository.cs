using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.License;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class LicenseRepository
{
    private readonly ILicenseClient _licenseClient;
    private readonly IOfflineService _offlineService;

    public LicenseRepository(ILicenseClient licenseClient, IOfflineService offlineService)
    {
        _licenseClient = licenseClient;
        _offlineService = offlineService;
    }
    
    
    public async Task<GetAllLicensesResponse?> GetAllAsync(bool forceCache, bool cachedAndReplace, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format("licenses"),
            async () => await _licenseClient.GetLicenseForCurrentCustomerAsync(clt), 
            new ApiCachedRequest { OneCallPerSession = true, ForceCache = forceCache, CachedAndReplace = cachedAndReplace},
            clt: clt);
        return response;
    }
}