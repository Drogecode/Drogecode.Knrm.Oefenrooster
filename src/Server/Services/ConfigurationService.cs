using System.Diagnostics;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Nager.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly Database.DataContext _database;
    private readonly IPreComService _preComService;

    public ConfigurationService(
        ILogger<ConfigurationService> logger, 
        Database.DataContext database, 
        IPreComService preComService)
    {
        _logger = logger;
        _database = database;
        _preComService = preComService;
    }

    public async Task<bool> UpgradeDatabase()
    {
        await _database.Database.MigrateAsync();

        return true;
    }

    public async Task<bool> AddSpecialDay(Guid customerId, PublicHoliday holiday, CancellationToken clt)
    {
        if (holiday.LocalName == null) return false;
        var date = DateTime.SpecifyKind(holiday.Date, DateTimeKind.Utc);
        var d = await _database.RoosterItemDays.FirstOrDefaultAsync(x =>
            x.CustomerId == customerId && x.Type == CalendarItemType.SpecialDate && x.IsFullDay == true && x.DateStart == date && x.Text == holiday.LocalName);
        if (d is not null) return false;
        await _database.RoosterItemDays.AddAsync(new DbRoosterItemDay
        {
            Id = Guid.NewGuid(),
            Type = CalendarItemType.SpecialDate,
            CustomerId = customerId,
            DateStart = date,
            Text = holiday.LocalName,
            IsFullDay = true
        });
        return await _database.SaveChangesAsync(clt) > 0;
    }

    public async Task<DbCorrectionResponse> DbCorrection(CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new DbCorrectionResponse();
        var success = true;
        var count = 0;

        var dbAlerts = await _database.PreComAlerts.Where(x=>x.Alert == "No alert found by hui.nu webhook" && x.UserId != null && x.Raw != null).ToListAsync(clt);
        if (dbAlerts.Count > 0)
        {
            foreach (var alert in dbAlerts)
            {
                var message = _preComService.AnalyzeAlert(alert.UserId.Value, alert.CustomerId.Value, JsonSerializer.Deserialize<object>(alert.Raw), out DateTime timestamp, out int? priority);
                alert.Alert = message;
                alert.Priority = priority;
                alert.SendTime = timestamp;
                if (!await _preComService.PatchAlertToDb(alert) || clt.IsCancellationRequested)
                {
                    success = false;
                    result.Message = $"Not all alerts fixed, stopped after `{count}`, see log in azure";
                    break;
                }
                count++;
            }
        }
        else
        {
            result.Message = "No valid alerts found.";
            success = false;
        }

        if (success)
        {
            result.Message = $"Fixed `{count}` alerts";
        }

        var linksFixed = 0;
        var dbLinks = await _database.LinkVehicleTraining.ToListAsync(clt);
        foreach (var link in dbLinks)
        {
            var dbTr = await _database.RoosterTrainings.FindAsync(link.RoosterTrainingId, clt);
            if (dbTr is null)
            {
                _database.LinkVehicleTraining.Remove(link);
                linksFixed++;
            }

            var dbVeh = await _database.Vehicles.FindAsync(link.VehicleId, clt);
            if (dbVeh is null)
            {
                _database.LinkVehicleTraining.Remove(link);
                linksFixed++;
            }
        }

        await _database.SaveChangesAsync(clt);
        result.Message += $" And `{linksFixed}` links fixed";
        _logger.LogInformation("Fixed `{count}` alerts And `{linksFixed}` links fixed", count, linksFixed);

        result.Success = success;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}