using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class CustomerSettingRepository
{
    private readonly ICustomerSettingsClient _customerSettingsClient;
    private readonly IOfflineService _offlineService;
    private const string TIMEZONE = "timezone";

    public CustomerSettingRepository(ICustomerSettingsClient customerSettingsClient, IOfflineService offlineService)
    {
        _customerSettingsClient = customerSettingsClient;
        _offlineService = offlineService;
    }

    public async Task<string> GetTimeZone(CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(TIMEZONE),
            async () => await _customerSettingsClient.GetTimeZoneAsync(clt), new ApiCachedRequest{OneCallPerSession = true},
            clt: clt);
        return response ?? "Europe/Amsterdam";
    }
}