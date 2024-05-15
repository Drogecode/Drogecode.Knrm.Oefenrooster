using System.Diagnostics;
using System.Text.Json;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Nager.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly Database.DataContext _database;

    public ConfigurationService(ILogger<ConfigurationService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
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

        var dbLinks = await _database.LinkVehicleTraining.ToListAsync(clt);
        foreach (var link in dbLinks)
        {
            var dbTr = await _database.RoosterTrainings.FindAsync(link.RoosterTrainingId, clt);
            if (dbTr is null)
            {
                _database.LinkVehicleTraining.Remove(link);
            }

            var dbVeh = await _database.Vehicles.FindAsync(link.VehicleId, clt);
            if (dbVeh is null)
            {
                _database.LinkVehicleTraining.Remove(link);
            }
        }

        result.Success = await _database.SaveChangesAsync(clt) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}