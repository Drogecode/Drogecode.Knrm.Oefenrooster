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
}
