using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class VehicleService :DrogeService,  IVehicleService
{

    public VehicleService(ILogger<VehicleService> logger, DataContext database, IMemoryCache memoryCache, IDateTimeService dateTimeService, IUserService userService) : base(logger, database,
        memoryCache, dateTimeService)
    {
    }

    public async Task<MultipleVehicleResponse> GetAllVehicles(Guid customerId)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleVehicleResponse()
        {
            DrogeVehicles = []
        };
        var dbVehicles = Database.Vehicles.Where(u => u.CustomerId == customerId && u.DeletedOn == null).OrderBy(x => x.Order);
        foreach (var dbVehicle in dbVehicles)
        {
            result.DrogeVehicles.Add(new DrogeVehicle
            {
                Id = dbVehicle.Id,
                Name = dbVehicle.Name,
                Code = dbVehicle.Code,
                Order = dbVehicle.Order,
                IsDefault = dbVehicle.IsDefault,
                IsActive = dbVehicle.IsActive,
            });
        }

        result.Success = true;
        result.TotalCount = result.DrogeVehicles.Count;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;

        return result;
    }

    public async Task<MultipleVehicleTrainingLinkResponse> GetForTraining(Guid customerId, Guid trainingId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleVehicleTrainingLinkResponse
        {
            DrogeLinkVehicleTrainingLinks = []
        };
        var dbVehicles = await Database.LinkVehicleTraining.Where(x => x.CustomerId == customerId && x.RoosterTrainingId == trainingId).ToListAsync(clt);
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
        var dbVehicles = await Database.RoosterDefaults.Where(x => x.CustomerId == customerId && x.Id == defaultId).Select(x => x.VehicleIds).FirstOrDefaultAsync(clt);
        if (dbVehicles == null) return result;
        var defaultSelectedVehicles = await Database.Vehicles.Where(x => x.IsDefault).Select(x => x.Id).ToListAsync(clt);
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
        dbVehicle.CreatedOn = DateTime.UtcNow;
        dbVehicle.Createdby = userId;
        dbVehicle.CustomerId = customerId;
        Database.Vehicles.Add(dbVehicle);
        if ((await Database.SaveChangesAsync(clt)) > 0)
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

        var oldVehicle = await Database.Vehicles.Where(x => x.CustomerId == customerId && x.Id == vehicle.Id && x.DeletedOn == null).FirstOrDefaultAsync(clt);
        if (oldVehicle is not null)
        {
            oldVehicle.Name = vehicle.Name;
            oldVehicle.Code = vehicle.Code;
            oldVehicle.Order = vehicle.Order;
            oldVehicle.IsDefault = vehicle.IsDefault;
            oldVehicle.IsActive = vehicle.IsActive;
            Database.Vehicles.Update(oldVehicle);
            result.Success = (await Database.SaveChangesAsync(clt)) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<DrogeLinkVehicleTrainingResponse> UpdateLinkVehicleTraining(Guid customerId, DrogeLinkVehicleTraining link)
    {
        var sw = Stopwatch.StartNew();
        var result = new DrogeLinkVehicleTrainingResponse();
        var foundLinks = await Database.LinkVehicleTraining.Where(x => x.CustomerId == customerId && x.VehicleId == link.VehicleId && x.RoosterTrainingId == link.RoosterTrainingId).ToListAsync();
        if (foundLinks.Count == 0)
        {
            var newId = Guid.NewGuid();
            Database.LinkVehicleTraining.Add(new DbLinkVehicleTraining
            {
                Id = newId,
                IsSelected = link.IsSelected,
                RoosterTrainingId = link.RoosterTrainingId,
                CustomerId = customerId,
                VehicleId = link.VehicleId,
            });
            link.Id = newId;
        }
        else
        {
            var first = true;
            foreach (var foundLink in foundLinks)
            {
                if (first)
                {
                    first = false;
                    link.Id = foundLink.Id;
                    foundLink.IsSelected = link.IsSelected;
                    Database.LinkVehicleTraining.Update(foundLink);
                }
                else
                {
                    Database.LinkVehicleTraining.Remove(foundLink);
                }
            }
        }
        result.DrogeLinkVehicleTraining = link;
        result.Success = (await Database.SaveChangesAsync()) > 0;

        if (!link.IsSelected)
        {
            var trainings = Database.RoosterAvailables.Where(x => x.TrainingId == link.RoosterTrainingId && x.VehicleId == link.VehicleId);
            foreach (var training in trainings)
            {
                training.VehicleId = null;
                Database.RoosterAvailables.Update(training);
            }
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}