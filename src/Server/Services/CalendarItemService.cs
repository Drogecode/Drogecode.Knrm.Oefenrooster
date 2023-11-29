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

    public async Task<GetMultipleMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken clt)
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
        var result = new GetMultipleMonthItemResponse
        {
            MonthItems = monthItems,
        };
        return result;
    }

    public async Task<GetMultipleDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, Guid userId, CancellationToken clt)
    {
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var dayItems = await _database.RoosterItemDays
            .Include(x => x.LinkUserDayItems)
            .Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.DateStart >= startDate && x.DateStart <= tillDate && (userId == Guid.Empty || x.LinkUserDayItems == null || x.LinkUserDayItems.Count == 0 || x.LinkUserDayItems.Any(x => x.UserForeignKey == userId)))
            .OrderBy(x => x.DateStart)
            .Select(x => x.ToRoosterItemDay())
            .ToListAsync(clt);
        var result = new GetMultipleDayItemResponse
        {
            DayItems = dayItems
        };
        return result;
    }

    public async Task<GetMultipleDayItemResponse> GetAllFutureDayItems(Guid customerId, int count, int skip, CancellationToken clt)
    {
        var startDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        var dayItems = await _database.RoosterItemDays
            .Include(x => x.LinkUserDayItems)
            .Where(x => x.CustomerId == customerId && x.DeletedOn == null && x.DateStart >= startDate)
            .OrderBy(x => x.DateStart)
            .Select(x => x.ToRoosterItemDay())
            .Skip(skip)
            .Take(count)
            .ToListAsync(clt);
        var result = new GetMultipleDayItemResponse
        {
            DayItems = dayItems
        };
        return result;
    }

    public async Task<GetDayItemResponse> GetDayItemById(Guid customerId, Guid id, CancellationToken clt = default)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetDayItemResponse();
        var dayItem = await _database.RoosterItemDays.FirstOrDefaultAsync(x => x.Id == id && x.DeletedOn == null && x.CustomerId == customerId);
        if (dayItem is null)
        {
            result.Success = false;
        }
        else
        {
            result.Success = true;
            result.DayItem = dayItem.ToRoosterItemDay();
        }
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
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
        if (roosterItemDay.UserIds is not null)
        {
            foreach (var usr in roosterItemDay.UserIds)
            {
                var dbLink = new DbLinkUserDayItem
                {
                    UserForeignKey = usr,
                    DayItemForeignKey = dbItem.Id
                };
                _database.LinkUserDayItems.Add(dbLink);
            }
        }
        result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        result.NewId = dbItem.Id;
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<PatchDayItemResponse> PatchDayItem(RoosterItemDay roosterItemDay, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new PatchDayItemResponse();
        var dayItem = await _database.RoosterItemDays.Include(x => x.LinkUserDayItems).FirstOrDefaultAsync(x => x.Id == roosterItemDay.Id && x.CustomerId == customerId && x.DeletedOn == null);
        if (dayItem is null)
        {
            result.Success = false;
        }
        else
        {
            dayItem.Text = roosterItemDay.Text;
            dayItem.DateStart = roosterItemDay.DateStart;
            dayItem.DateEnd = roosterItemDay.DateEnd;
            dayItem.IsFullDay = roosterItemDay.IsFullDay;
            dayItem.Type = roosterItemDay.Type;
            _database.RoosterItemDays.Update(dayItem);
            if (roosterItemDay.UserIds is not null)
            {
                foreach (var usr in roosterItemDay.UserIds)
                {
                    if (dayItem.LinkUserDayItems?.Any(x => x.UserForeignKey == usr) is true)
                        continue;
                    var dbLink = new DbLinkUserDayItem
                    {
                        UserForeignKey = usr,
                        DayItemForeignKey = dayItem.Id
                    };
                    _database.LinkUserDayItems.Add(dbLink);
                }
                if (dayItem.LinkUserDayItems?.Any() is true)
                {
                    _database.LinkUserDayItems.RemoveRange(dayItem.LinkUserDayItems.Where(x => !roosterItemDay.UserIds.Contains(x.UserForeignKey)));
                }
            }
            else if (dayItem.LinkUserDayItems is not null)
                _database.LinkUserDayItems.RemoveRange(dayItem.LinkUserDayItems);
            result.Success = (await _database.SaveChangesAsync(clt)) > 0;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }


    public async Task<bool> DeleteDayItem(Guid idToDelete, Guid customerId, Guid userId, CancellationToken clt)
    {
        var item = await _database.RoosterItemDays.FirstOrDefaultAsync(x => x.Id == idToDelete && x.CustomerId == customerId && x.DeletedOn == null);
        if (item is null)
            return false;
        item.DeletedOn = DateTime.UtcNow;
        item.DeletedBy = userId;
        _database.RoosterItemDays.Update(item);
        return (await _database.SaveChangesAsync(clt)) > 0;
    }
}
