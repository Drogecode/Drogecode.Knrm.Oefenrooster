using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class VehicleRepository
{
    private readonly IVehicleClient _vehicleClient;
    private readonly IOfflineService _offlineService;
    private const string VEHICLES = "vehicles";

    public VehicleRepository(IVehicleClient vehicleClient, IOfflineService offlineService)
    {
        _vehicleClient = vehicleClient;
        _offlineService = offlineService;
    }

    public async Task<List<DrogeVehicle>?> GetAllVehiclesAsync(bool cachedAndReplace, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(VEHICLES,
            async () => await _vehicleClient.GetAllAsync(cachedAndReplace, clt), 
            new ApiCachedRequest { OneCallPerSession = true, CachedAndReplace = cachedAndReplace},
            clt: clt);
        return response?.DrogeVehicles?.ToList();
    }

    public async Task<List<DrogeLinkVehicleTraining>?> GetForTrainingAsync(Guid trainingId)
    {
        var dbVehicle = await _vehicleClient.GetForTrainingAsync(trainingId);
        return dbVehicle.DrogeLinkVehicleTrainingLinks?.ToList();
    }

    public async Task<List<DrogeLinkVehicleTraining>?> GetForDefaultAsync(Guid defaultId)
    {
        var dbVehicle = await _vehicleClient.GetForDefaultAsync(defaultId);
        return dbVehicle.DrogeLinkVehicleTrainingLinks?.ToList();
    }

    public async Task<DrogeLinkVehicleTraining?> UpdateLinkVehicleTrainingAsync(DrogeLinkVehicleTraining link)
    {
        if (link.RoosterTrainingId == Guid.Empty)
        {
            throw new ArgumentNullException($"link.RoosterTrainingId is empty");
        }
        var successfull = await _vehicleClient.UpdateLinkVehicleTrainingAsync(link);
        return successfull?.DrogeLinkVehicleTraining;
    }
}
