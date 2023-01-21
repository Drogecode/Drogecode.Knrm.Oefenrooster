using static System.Net.WebRequestMethods;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class UserRepository
{
    private readonly HttpClient _httpClient;

    public UserRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DrogeUser?> GetCurrentUserAsync()
    {
        var dbUser = await _httpClient.GetFromJsonAsync<DrogeUser>("api/User/Get");
        return dbUser;
    }
}
