using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using System;
using System.Net.Http.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ConfigurationRepository
{
    private readonly HttpClient _httpClient;

    public ConfigurationRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task UpgradeDatabaseAsync()
    {
        var response = await _httpClient.GetAsync("api/Configuration/UpgradeDatabase");
        response.EnsureSuccessStatusCode();
    }

    public async Task<bool> NewVersionAvailable()
    {
        var clientVersion = DefaultSettingsHelper.CURRENT_VERSION;
        var response = await _httpClient.GetFromJsonAsync<bool>($"api/Configuration/NewVersionAvailable?clientVersion={clientVersion}");
        return response;
    }
}
