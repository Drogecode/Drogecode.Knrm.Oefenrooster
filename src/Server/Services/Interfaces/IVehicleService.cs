using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IVehicleService
{
    Task<List<DrogeVehicle>> GetAllVehicles(Guid customerId);
    Task<List<DrogeLinkVehicleTraining>> GetForTraining(Guid customerId, Guid trainingId);
    Task<DrogeLinkVehicleTraining> UpdateLinkVehicleTraining(Guid customerId, DrogeLinkVehicleTraining link);
}
