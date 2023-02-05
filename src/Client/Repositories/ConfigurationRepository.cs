using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using System;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

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

    public async Task<UpdateDetails?> NewVersionAvailable()
    {
        var clientVersion = DefaultSettingsHelper.CURRENT_VERSION;
        var response = await _httpClient.GetFromJsonAsync<UpdateDetails?>($"api/Configuration/NewVersionAvailable?clientVersion={WebUtility.UrlEncode(clientVersion)}");
        return response;
    }
}
