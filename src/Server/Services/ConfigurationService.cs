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

    public async Task<bool> AddSpecialDay(Guid customerId, PublicHoliday holiday, CancellationToken token)
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
        return _database.SaveChanges() > 0;
    }

    public async Task<DbCorrectionResponse> DbCorrection1(CancellationToken clt)
    {
        var result = new DbCorrectionResponse();
        var sw = Stopwatch.StartNew();

        var audits = await _database.Audits.Where(x => x.AuditType == AuditType.PatchAssignedUser && x.Note.Contains("Availabilty")).ToListAsync(clt);
        foreach (var audit in audits.Where(audit => !string.IsNullOrEmpty(audit.Note)))
        {
            var note = JsonSerializer.Deserialize<AuditAssignedUser>(audit.Note!);
            if (note is not null && note.Availability is null && note.Availabilty is not null)
            {
                note.Availability = (Availability)(int)note.Availabilty.Value;
                audit.Note = JsonSerializer.Serialize(note);
                _database.Audits.Update(audit);
            }
        }

        result.Success = await _database.SaveChangesAsync(clt) > 0;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}