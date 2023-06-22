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
                Description = dbHoliday.Description,
                Available = dbHoliday.Available,
                ValidFrom = dbHoliday.ValidFrom,
                ValidUntil = dbHoliday.ValidUntil,
            });
        }
        return list;
    }

    public async Task<PutHolidaysForUserResponse> PutHolidaysForUser(Holiday body, Guid customerId, Guid userId)
    {
        if (body is null) return new PutHolidaysForUserResponse { Success = false };
        if (body.ValidUntil is not null && body.ValidUntil.Value.CompareTo(DateTime.Today) <= 0) return new PutHolidaysForUserResponse { Success = false };
        if (body.ValidFrom is not null && body.ValidFrom.Value.CompareTo(DateTime.Today) < 0) body.ValidFrom = DateTime.Today;
        if (body!.ValidUntil!.Value.CompareTo(body.ValidFrom) < 0) return new PutHolidaysForUserResponse { Success = false };
        var dbHoliday = new DbUserHolidays
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Description= body.Description,
            CustomerId = customerId,
            Available = body.Available,
            ValidFrom = body.ValidFrom!.Value,
            ValidUntil = body.ValidUntil!.Value
        };
        body.Id = dbHoliday.Id;
        body.ValidFrom = dbHoliday.ValidFrom;
        _database.UserHolidays.Add(dbHoliday);
        _database.SaveChanges();
        return new PutHolidaysForUserResponse
        {
            Success = true,
            Put = body
        };
    }

    public async Task<PatchHolidaysForUserResponse> PatchHolidaysForUser(Holiday body, Guid customerId, Guid userId)
    {
        var dbHoliday = _database.UserHolidays.FirstOrDefault(x => x.Id == body.Id);
        if (dbHoliday is null) return new PatchHolidaysForUserResponse { Success = false };
        if (dbHoliday.ValidUntil is not null && dbHoliday.ValidUntil.Value.CompareTo(DateTime.UtcNow) <= 0) return new PatchHolidaysForUserResponse { Success = false };
        if (dbHoliday.ValidFrom is not null && dbHoliday.ValidFrom.Value.CompareTo(DateTime.UtcNow) <= 0) body.ValidFrom = dbHoliday.ValidFrom;

        dbHoliday.Description = body.Description;
        dbHoliday.Available = body.Available;
        dbHoliday.ValidFrom = body.ValidFrom!.Value;
        dbHoliday.ValidUntil = body.ValidUntil!.Value;
        _database.UserHolidays.Update(dbHoliday);

        _database.SaveChanges();
        return new PatchHolidaysForUserResponse
        {
            Success = true,
            Patched = body
        };
    }
}
