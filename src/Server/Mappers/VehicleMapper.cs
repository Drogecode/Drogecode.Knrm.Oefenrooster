using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Server.Mappers;

public static class VehicleMapper
{
    public static DbVehicles ToDb(this DrogeVehicle vehicle)
    {
        return new DbVehicles
        {
            Id = vehicle.Id,
            Name = vehicle.Name,
            Code = vehicle.Code,    
            Order = vehicle.Order,
            IsDefault   = vehicle.IsDefault,
            IsActive = vehicle.IsActive
        };
    }
    public static DrogeVehicle ToDrogecode(this DbVehicles vehicle)
    {
        return new DrogeVehicle
        {
            Id = vehicle.Id,
            Name = vehicle.Name,
            Code = vehicle.Code,    
            Order = vehicle.Order,
            IsDefault   = vehicle.IsDefault,
            IsActive = vehicle.IsActive
        };
    }
}
