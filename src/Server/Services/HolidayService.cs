using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
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

    public async Task<List<Holiday>> GetAllHolidaysForUser(Guid customerId, Guid userId, CancellationToken clt)
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

    public async Task<GetResponse> Get(Guid id, Guid customerId, Guid userId, CancellationToken clt)
    {
        var result = new GetResponse();
        var dbHoliday = await _database.UserHolidays.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId && x.UserId == userId, clt);
        if (dbHoliday is null) return result;
        result.Holiday = new Holiday
        {
            Id = dbHoliday.Id,
            UserId = userId,
            Description = dbHoliday.Description,
            Available = dbHoliday.Available,
            ValidFrom = dbHoliday.ValidFrom,
            ValidUntil = dbHoliday.ValidUntil,
        };
        result.Success = true;
        return result;
    }

    public async Task<PutHolidaysForUserResponse> PutHolidaysForUser(Holiday body, Guid customerId, Guid userId, CancellationToken clt)
    {
        if (body is null) return new PutHolidaysForUserResponse { Success = false };
        if (body.ValidUntil is not null && body.ValidUntil.Value.CompareTo(DateTime.Today) <= 0) return new PutHolidaysForUserResponse { Success = false };
        if (body.ValidFrom is not null && body.ValidFrom.Value.CompareTo(DateTime.Today) < 0) body.ValidFrom = DateTime.UtcNow;
        if (body!.ValidUntil!.Value.CompareTo(body.ValidFrom) < 0) return new PutHolidaysForUserResponse { Success = false };
        var dbHoliday = body.ToDbHoliday();
        dbHoliday.Id = Guid.NewGuid();
        dbHoliday.UserId = userId;
        dbHoliday.CustomerId = customerId;
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

    public async Task<PatchHolidaysForUserResponse> PatchHolidaysForUser(Holiday body, Guid customerId, Guid userId, CancellationToken clt)
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

        await _database.SaveChangesAsync(clt);
        return new PatchHolidaysForUserResponse
        {
            Success = true,
            Patched = body
        };
    }

    public async Task<DeleteResonse> Delete(Guid id, Guid customerId, Guid userId, CancellationToken clt)
    {
        var result = new DeleteResonse();
        var dbHoliday = await _database.UserHolidays.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId && x.UserId == userId, clt);
        if (dbHoliday?.ValidUntil is null || dbHoliday.ValidUntil.Value.CompareTo(DateTime.UtcNow) <= 0) return result;
        if (dbHoliday.ValidFrom is not null && dbHoliday.ValidFrom.Value.CompareTo(DateTime.UtcNow) <= 0)
        {
            dbHoliday.ValidUntil = DateTime.UtcNow;
            _database.UserHolidays.Update(dbHoliday);
        }
        else
        {
            _database.UserHolidays.Remove(dbHoliday);
        }
        result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        return result;
    }
}
