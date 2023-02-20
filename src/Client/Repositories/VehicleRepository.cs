namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class VehicleRepository
{
    private readonly HttpClient _httpClient;

    public VehicleRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DrogeVehicle>?> GetAllVehiclesAsync()
    {
        var dbVehicle = await _httpClient.GetFromJsonAsync<List<DrogeVehicle>>("api/Vehicle/GetAll");
        return dbVehicle;
    }
}
