using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;
using Drogecode.Knrm.Oefenrooster.Shared.Services.Interfaces;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class HolidayService : IHolidayService
{
    private readonly ILogger<HolidayService> _logger;
    private readonly DataContext _database;
    private readonly IDateTimeService _dateTimeService;
    public HolidayService(ILogger<HolidayService> logger,
        DataContext database,
        IDateTimeService dateTimeService)
    {
        _logger = logger;
        _database = database;
        _dateTimeService = dateTimeService;
    }

    public async Task<MultipleHolidaysResponse> GetAllHolidaysForUser(Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleHolidaysResponse();
        var list = new List<Holiday>();
        var dbHolidays = _database.UserHolidays.Where(y => y.CustomerId == customerId && y.UserId == userId);
        foreach (var dbHoliday in await dbHolidays.ToListAsync(clt))
        {
            if (dbHoliday is null) continue;
            list.Add(new Holiday
            {
                Id = dbHoliday.Id,
                UserId = dbHoliday.UserId,
                Description = dbHoliday.Description,
                Availability = dbHoliday.Available,
                ValidFrom = dbHoliday.ValidFrom,
                ValidUntil = dbHoliday.ValidUntil,
            });
        }
        result.Holidays = list;
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<MultipleHolidaysResponse> GetAllHolidaysForFuture(Guid customerId, Guid userId, int days, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new MultipleHolidaysResponse();
        var list = new List<Holiday>();
        var dbHolidays = _database.UserHolidays.Where(y => y.CustomerId == customerId && y.ValidUntil >= _dateTimeService.UtcNow());
        result.TotalCount = dbHolidays.Count();
        foreach (var dbHoliday in await dbHolidays.Where(y => y.ValidFrom <= _dateTimeService.UtcNow().AddDays(days)).OrderBy(x => x.ValidFrom).ToListAsync(clt))
        {
            if (dbHoliday is null) continue;
            list.Add(new Holiday
            {
                Id = dbHoliday.Id,
                UserId = dbHoliday.UserId,
                Description = dbHoliday.Description,
                Availability = dbHoliday.Available,
                ValidFrom = dbHoliday.ValidFrom,
                ValidUntil = dbHoliday.ValidUntil,
            });
        }
        result.Holidays = list;
        result.Success = true;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
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
            Availability = dbHoliday.Available,
            ValidFrom = dbHoliday.ValidFrom,
            ValidUntil = dbHoliday.ValidUntil,
        };
        result.Success = true;
        return result;
    }

    public async Task<PutHolidaysForUserResponse> PutHolidaysForUser(Holiday body, Guid customerId, Guid userId, CancellationToken clt)
    {
        if (body is null) return new PutHolidaysForUserResponse { Success = false };
        if (body.ValidUntil is not null && body.ValidUntil.Value.CompareTo(_dateTimeService.Today()) <= 0) return new PutHolidaysForUserResponse { Success = false };
        if (body.ValidFrom is not null && body.ValidFrom.Value.CompareTo(_dateTimeService.Today()) < 0) body.ValidFrom = _dateTimeService.UtcNow();
        if (body!.ValidUntil!.Value.CompareTo(body.ValidFrom) < 0) return new PutHolidaysForUserResponse { Success = false };
        var dbHoliday = body.ToDbHoliday();
        dbHoliday.Id = Guid.NewGuid();
        dbHoliday.UserId = userId;
        dbHoliday.CustomerId = customerId;
        body.Id = dbHoliday.Id;
        body.ValidFrom = dbHoliday.ValidFrom;
        _database.UserHolidays.Add(dbHoliday);
        await _database.SaveChangesAsync(clt);
        return new PutHolidaysForUserResponse
        {
            Success = true,
            Put = body
        };
    }

    public async Task<PatchHolidaysForUserResponse> PatchHolidaysForUser(Holiday body, Guid customerId, Guid userId, CancellationToken clt)
    {
        var dbHoliday = await _database.UserHolidays.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.Id == body.Id, cancellationToken: clt);
        if (dbHoliday is null) return new PatchHolidaysForUserResponse { Success = false };
        if (dbHoliday.ValidUntil is not null && dbHoliday.ValidUntil.Value.CompareTo(_dateTimeService.UtcNow()) <= 0) return new PatchHolidaysForUserResponse { Success = false };
        if (dbHoliday.ValidFrom is not null && dbHoliday.ValidFrom.Value.CompareTo(_dateTimeService.UtcNow()) <= 0) body.ValidFrom = dbHoliday.ValidFrom;

        dbHoliday.Description = body.Description;
        dbHoliday.Available = body.Availability;
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

    public async Task<DeleteResponse> Delete(Guid id, Guid customerId, Guid userId, CancellationToken clt)
    {
        var result = new DeleteResponse();
        var dbHoliday = await _database.UserHolidays.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == customerId && x.UserId == userId, clt);
        if (dbHoliday?.ValidUntil is null || dbHoliday.ValidUntil.Value.CompareTo(_dateTimeService.UtcNow()) <= 0) return result;
        if (dbHoliday.ValidFrom is not null && dbHoliday.ValidFrom.Value.CompareTo(_dateTimeService.UtcNow()) <= 0)
        {
            dbHoliday.ValidUntil = _dateTimeService.UtcNow();
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
