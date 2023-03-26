using static System.Net.WebRequestMethods;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class UserRepository
{
    private readonly HttpClient _httpClient;

    public UserRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DrogeUser>?> GetAllUsersAsync()
    {
        var dbUser = await _httpClient.GetFromJsonAsync<List<DrogeUser>>("api/User/GetAll");
        return dbUser;
    }
    public async Task<DrogeUser?> GetCurrentUserAsync()
    {
        var dbUser = await _httpClient.GetFromJsonAsync<DrogeUser>("api/User/Get");
        return dbUser;
    }
    public async Task<bool> UpdateUserAsync(DrogeUser user)
    {
        var request = await _httpClient.PostAsJsonAsync("api/User/UpdateUser", user);
        var successfull = await request.Content.ReadFromJsonAsync<bool>();
        return successfull;
    }
    public async Task<bool> AddUserAsync(DrogeUser user)
    {
        var request = await _httpClient.PostAsJsonAsync("api/User/AddUser", user);
        var successfull = await request.Content.ReadFromJsonAsync<bool>();
        return successfull;
    }
    public async Task<bool> SyncAllUsersAsync()
    {
        var request = await _httpClient.GetFromJsonAsync<bool>("api/User/SyncAllUsers");
        return request;
    }
}
