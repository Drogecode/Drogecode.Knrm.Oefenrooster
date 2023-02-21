namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class VehicleService : IVehicleService
{
    private readonly ILogger<VehicleService> _logger;
    private readonly Database.DataContext _database;
    public VehicleService(ILogger<VehicleService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<List<DrogeVehicle>> GetAllVehicles(Guid customerId)
    {
        var result = new List<DrogeVehicle>();
        var dbVehicles = _database.Vehicles.Where(u => u.CustomerId == customerId && u.DeletedOn == null).OrderBy(x => x.Order);
        foreach (var dbVehicle in dbVehicles)
        {
            result.Add(new DrogeVehicle
            {
                Id = dbVehicle.Id,
                Name = dbVehicle.Name,
                Code = dbVehicle.Code,
                Order = dbVehicle.Order,
                Default = dbVehicle.Default,
                Active = dbVehicle.Active,
            });
        }
        return result;
    }

    public async Task<List<DrogeLinkVehicleTraining>> GetForTraining(Guid customerId, Guid trainingId)
    {
        var result = new List<DrogeLinkVehicleTraining>();
        var dbVehicles = _database.LinkVehicleTraining.Where(x => x.RoosterTrainingId == trainingId).ToList();
        foreach (var dbVehicle in dbVehicles)
        {
            result.Add(new DrogeLinkVehicleTraining
            {
                RoosterTrainingId = dbVehicle.RoosterTrainingId,
                Vehicle = dbVehicle.Vehicle,
                IsSelected= dbVehicle.IsSelected,
            });
        }
        return result;
    }
}
