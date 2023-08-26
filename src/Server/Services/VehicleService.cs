using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Graph.Models;
using System.Diagnostics;

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
                IsDefault = dbVehicle.IsDefault,
                IsActive = dbVehicle.IsActive,
            });
        }
        return result;
    }

    public async Task<List<DrogeLinkVehicleTraining>> GetForTraining(Guid customerId, Guid trainingId)
    {
        var result = new List<DrogeLinkVehicleTraining>();
        var dbVehicles = _database.LinkVehicleTraining.Where(x => x.CustomerId == customerId && x.RoosterTrainingId == trainingId).ToList();
        foreach (var dbVehicle in dbVehicles)
        {
            result.Add(new DrogeLinkVehicleTraining
            {
                Id = dbVehicle.Id,
                RoosterTrainingId = dbVehicle.RoosterTrainingId,
                VehicleId = dbVehicle.VehicleId,
                IsSelected = dbVehicle.IsSelected,
            });
        }
        return result;
    }

    public async Task<Guid?> PutVehicle(DrogeVehicle vehicle, Guid customerId, Guid userId, CancellationToken clt)
    {
        vehicle.Id = Guid.NewGuid();
        var dbVehicle = vehicle.ToDb();
        dbVehicle.CreatedOn = DateTime.Now;
        dbVehicle.Createdby = userId;
        dbVehicle.CustomerId = customerId;
        _database.Vehicles.Add(dbVehicle);
        if ((await _database.SaveChangesAsync()) > 0)
            return vehicle.Id;
        return null;
    }

    public async Task<DrogeLinkVehicleTrainingResponse> UpdateLinkVehicleTraining(Guid customerId, DrogeLinkVehicleTraining link)
    {
        var sw = Stopwatch.StartNew();
        var result = new DrogeLinkVehicleTrainingResponse();
        if (link.Id == null)
        {
            var newId = Guid.NewGuid();
            _database.LinkVehicleTraining.Add(new DbLinkVehicleTraining
            {
                Id = newId,
                IsSelected = link.IsSelected,
                RoosterTrainingId = link.RoosterTrainingId,
                CustomerId = customerId,
                VehicleId = link.VehicleId,
            });
            result.Success = (await _database.SaveChangesAsync()) > 0;
            link.Id = newId;
            result.DrogeLinkVehicleTraining = link;
        }
        else
        {
            var foundLink = await _database.LinkVehicleTraining.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == link.Id);
            if (foundLink == null)
            {
                _logger.LogWarning("Link {LinkId} was not found", link.Id);
                result.DrogeLinkVehicleTraining = link;
            }
            else
            {
                foundLink.IsSelected = link.IsSelected;
                _database.LinkVehicleTraining.Update(foundLink);
                if (!link.IsSelected)
                {
                    var trainings = _database.RoosterAvailables.Where(x => x.TrainingId == link.RoosterTrainingId && x.VehicleId == link.VehicleId);
                    foreach (var training in trainings)
                    {
                        training.VehicleId = null;
                        _database.RoosterAvailables.Update(training);
                    }
                }
                result.Success = (await _database.SaveChangesAsync()) > 0;
                result.DrogeLinkVehicleTraining = link;
            }
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}
