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

    public async Task<List<DrogeLinkVehicleTraining>?> GetForTrainingAsync(Guid trainingId)
    {

        var dbVehicle = await _httpClient.GetFromJsonAsync<List<DrogeLinkVehicleTraining>>($"api/Vehicle/GetForTraining?trainingId={trainingId}");
        return dbVehicle;
    }
}
