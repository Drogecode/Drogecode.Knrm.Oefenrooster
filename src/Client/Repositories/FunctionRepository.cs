using System.Net.Http;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class FunctionRepository
{
    private readonly HttpClient _httpClient;

    public FunctionRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DrogeFunction>?> GetAllFunctionsAsync()
    {
        var dbUser = await _httpClient.GetFromJsonAsync<List<DrogeFunction>>("api/Function/GetAll");
        return dbUser;
    }
}
