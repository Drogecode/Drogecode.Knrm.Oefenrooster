using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
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
        var d = await _database.RoosterItemDays.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Type == CalendarItemType.SpecialDate && x.IsFullDay == true && x.DateStart == date && x.Text == holiday.LocalName);
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
}
