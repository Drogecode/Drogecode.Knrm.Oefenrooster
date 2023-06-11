using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class VehicleRepository
{
    private readonly IVehicleClient _vehicleClient;

    public VehicleRepository(IVehicleClient vehicleClient)
    {
        _vehicleClient = vehicleClient;
    }

    public async Task<List<DrogeVehicle>?> GetAllVehiclesAsync()
    {
        var dbVehicle = await _vehicleClient.GetAllAsync();
        return dbVehicle.DrogeVehicles?.ToList();
    }

    public async Task<List<DrogeLinkVehicleTraining>?> GetForTrainingAsync(Guid trainingId)
    {

        var dbVehicle = await _vehicleClient.GetForTrainingAsync(trainingId);
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
