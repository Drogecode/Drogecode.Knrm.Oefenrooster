using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class HolidayService : IHolidayService
{
    private readonly ILogger<HolidayService> _logger;
    private readonly Database.DataContext _database;
    public HolidayService(ILogger<HolidayService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<List<Holiday>> GetAllHolidaysForUser(Guid customerId, Guid userId)
    {
        var list = new List<Holiday>();
        var dbHolidays = _database.UserHolidays.Where(y => y.UserId == userId);
        foreach (var dbHoliday in dbHolidays)
        {
            if (dbHoliday is null) continue;
            list.Add(new Holiday
            {
                Id = dbHoliday.Id,
                UserId = userId,
                Available = dbHoliday.Available,
                ValidFrom = dbHoliday.ValidFrom,
                ValidUntil = dbHoliday.ValidUntil,
            });
        }
        return list;
    }

    public async Task<PatchHolidaysForUserResponse> PatchHolidaysForUser(Holiday body, Guid customerId, Guid userId)
    {
        var dbHoliday = _database.UserHolidays.FirstOrDefault(x => x.Id == body.Id);
        if (dbHoliday is null || true) return new PatchHolidaysForUserResponse { Success = false }; //Not ready
        if (dbHoliday?.ValidFrom?.Date.Equals(DateTime.UtcNow.Date) == true)
        {
            dbHoliday!.Available = body.Available;
            dbHoliday.ValidFrom = body.ValidFrom;
            dbHoliday.ValidUntil = body.ValidUntil;
            _database.UserHolidays.Update(dbHoliday);
        }
        else
        {
            if (dbHoliday is not null)
            {
                dbHoliday.ValidUntil = DateTime.UtcNow.AddDays(-1);
                _database.UserHolidays.Update(dbHoliday);
            }
            dbHoliday = new DbUserHolidays
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CustomerId = customerId,
                Available = body.Available,
                ValidFrom = DateTime.UtcNow,
                ValidUntil = body.ValidUntil,
            };
            body.Id = dbHoliday.Id;
            body.ValidFrom = dbHoliday.ValidFrom;
            _database.UserHolidays.Add(dbHoliday);
        }
        _database.SaveChanges();
        return new PatchHolidaysForUserResponse
        {
            Success = true,
            Patched = body
        };
    }
}
