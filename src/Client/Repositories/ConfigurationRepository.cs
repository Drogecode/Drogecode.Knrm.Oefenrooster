using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using System.Net;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ConfigurationRepository
{
    private readonly HttpClient _httpClient;

    public ConfigurationRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> UpgradeDatabaseAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<bool>("api/Configuration/UpgradeDatabase");
        return response;    
    }

    public async Task<UpdateDetails?> NewVersionAvailable()
    {
        var clientVersion = DefaultSettingsHelper.CURRENT_VERSION;
        var response = await _httpClient.GetFromJsonAsync<UpdateDetails?>($"api/Configuration/NewVersionAvailable?clientVersion={WebUtility.UrlEncode(clientVersion)}");
        return response;
    }

    public async Task<bool?> InstallingActive()
    {
        var response = await _httpClient.GetFromJsonAsync<bool?>("api/Configuration/InstallingActive");
        return response;
    }
}
