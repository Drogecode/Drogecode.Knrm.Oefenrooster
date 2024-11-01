using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Graph.Models;
using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Shared;

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

    public async Task<MultipleVehicleTrainingLinkResponse> GetForTraining(Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleVehicleTrainingLinkResponse
        {
            DrogeLinkVehicleTrainingLinks = []
        };
        var dbVehicles = await _database.LinkVehicleTraining.Where(x => x.CustomerId == customerId && x.RoosterTrainingId == trainingId).ToListAsync(clt);
        foreach (var dbVehicle in dbVehicles)
        {
            result.DrogeLinkVehicleTrainingLinks.Add(new DrogeLinkVehicleTraining
            {
                Id = dbVehicle.Id,
                RoosterTrainingId = dbVehicle.RoosterTrainingId,
                VehicleId = dbVehicle.VehicleId,
                IsSelected = dbVehicle.IsSelected,
            });
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<MultipleVehicleTrainingLinkResponse> GetForDefault(Guid customerId, Guid defaultId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleVehicleTrainingLinkResponse
        {
            DrogeLinkVehicleTrainingLinks = []
        };
        var dbVehicles = await _database.RoosterDefaults.Where(x => x.CustomerId == customerId && x.Id == defaultId).Select(x => x.VehicleIds).FirstOrDefaultAsync(clt);
        if (dbVehicles == null) return result;
        var defaultSelectedVehicles = await _database.Vehicles.Where(x => x.IsDefault).Select(x => x.Id).ToListAsync(clt);
        foreach (var dbVehicle in dbVehicles)
        {
            result.DrogeLinkVehicleTrainingLinks.Add(new DrogeLinkVehicleTraining
            {
                VehicleId = dbVehicle,
                IsSelected = true,
            });
        }

        foreach (var defVehicle in defaultSelectedVehicles)
        {
            if (dbVehicles.Any(x => x == defVehicle)) continue;
            result.DrogeLinkVehicleTrainingLinks.Add(new DrogeLinkVehicleTraining
            {
                VehicleId = defVehicle,
                IsSelected = false,
            });
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PutResponse> PutVehicle(DrogeVehicle vehicle, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutResponse();
        vehicle.Id = Guid.NewGuid();
        var dbVehicle = vehicle.ToDb();
        dbVehicle.CreatedOn = DateTime.Now;
        dbVehicle.Createdby = userId;
        dbVehicle.CustomerId = customerId;
        _database.Vehicles.Add(dbVehicle);
        if ((await _database.SaveChangesAsync(clt)) > 0)
        {
            result.NewId = vehicle.Id;
            result.Success = true;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchResponse> PatchVehicle(DrogeVehicle vehicle, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchResponse();

        var oldVehicle = await _database.Vehicles.Where(x => x.CustomerId == customerId && x.Id == vehicle.Id && x.DeletedOn == null).FirstOrDefaultAsync(clt);
        if (oldVehicle is not null)
        {
            oldVehicle.Name = vehicle.Name;
            oldVehicle.Code = vehicle.Code;
            oldVehicle.Order = vehicle.Order;
            oldVehicle.IsDefault = vehicle.IsDefault;
            oldVehicle.IsActive = vehicle.IsActive;
            _database.Vehicles.Update(oldVehicle);
            result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<DrogeLinkVehicleTrainingResponse> UpdateLinkVehicleTraining(Guid customerId, DrogeLinkVehicleTraining link)
    {
        var sw = Stopwatch.StartNew();
        var result = new DrogeLinkVehicleTrainingResponse();
        if (link.Id is null)
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
            if (foundLink is null)
            {
                _logger.LogWarning("Link {LinkId} was not found", link.Id);
                result.DrogeLinkVehicleTraining = link;
            }
            else
            {
                foundLink.IsSelected = link.IsSelected;
                _database.LinkVehicleTraining.Update(foundLink);
                result.Success = (await _database.SaveChangesAsync()) > 0;
                result.DrogeLinkVehicleTraining = link;
            }
        }

        if (!link.IsSelected)
        {
            var trainings = _database.RoosterAvailables.Where(x => x.TrainingId == link.RoosterTrainingId && x.VehicleId == link.VehicleId);
            foreach (var training in trainings)
            {
                training.VehicleId = null;
                _database.RoosterAvailables.Update(training);
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}