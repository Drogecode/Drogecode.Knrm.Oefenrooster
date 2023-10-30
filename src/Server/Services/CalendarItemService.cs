using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

[EndpointGroupName("CalendarItem")]
public class CalendarItemService : ICalendarItemService
{
    private readonly ILogger<CalendarItemService> _logger;
    private readonly Database.DataContext _database;
    public CalendarItemService(ILogger<CalendarItemService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<GetMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken clt)
    {
        var monthItems = await _database.RoosterItemMonths.Where(x => x.CustomerId == customerId && x.Month == month && (x.Year == null || x.Year == 0 || x.Year == year)).Select(
            x => new RoosterItemMonth
            {
                Id = x.Id,
                Month = x.Month,
                Severity = x.Severity,
                Year = x.Year,
                Type = x.Type,
                Text = x.Text,
                Order = x.Order,
            }).ToListAsync(clt);
        var result = new GetMonthItemResponse
        {
            MonthItems = monthItems,
        };
        return result;
    }

    public async Task<GetDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, Guid userId, CancellationToken clt)
    {
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var dayItems = await _database.RoosterItemDays
            .Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate && (userId == Guid.Empty || x.UserId == Guid.Empty || x.UserId.Equals(userId)))
            .OrderBy(x => x.DateStart)
            .Select(x => x.ToRoosterItemDay())
            .ToListAsync(clt);
        var result = new GetDayItemResponse
        {
            DayItems = dayItems
        };
        return result;
    }

    public async Task<GetDayItemResponse> GetAllFutureDayItems(Guid customerId, int count, int skip, CancellationToken clt)
    {
        var startDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        var dayItems = await _database.RoosterItemDays
            .Where(x => x.CustomerId == customerId && x.DateStart >= startDate)
            .OrderBy(x => x.DateStart)
            .Select(x => x.ToRoosterItemDay())
            .Skip(skip)
            .Take(count)
            .ToListAsync(clt);
        var result = new GetDayItemResponse
        {
            DayItems = dayItems
        };
        return result;
    }

    public async Task<PutMonthItemResponse> PutMonthItem(RoosterItemMonth roosterItemMonth, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutMonthItemResponse();
        var dbItem = roosterItemMonth.ToDbRoosterItemMonth();
        dbItem.Id = Guid.NewGuid();
        dbItem.CustomerId = customerId;
        dbItem.CreatedBy = userId;
        dbItem.CreatedOn = DateTime.UtcNow;
        _database.RoosterItemMonths.Add(dbItem);
        result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        result.NewId = dbItem.Id;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PutDayItemResponse> PutDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PutDayItemResponse();
        var dbItem = roosterItemDay.ToDbRoosterItemDay();
        dbItem.Id = Guid.NewGuid();
        dbItem.CustomerId = customerId;
        dbItem.CreatedBy = userId;
        dbItem.CreatedOn = DateTime.UtcNow;
        _database.RoosterItemDays.Add(dbItem);
        result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        result.NewId = dbItem.Id;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }
}
