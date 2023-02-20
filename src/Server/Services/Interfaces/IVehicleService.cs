namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IVehicleService
{
    Task<List<DrogeVehicle>> GetAllVehicles(Guid customerId);
}
