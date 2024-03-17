using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Mappers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using System.Diagnostics;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class DayItemService : IDayItemService
{
    private readonly ILogger<DayItemService> _logger;
    private readonly Database.DataContext _database;
    public DayItemService(ILogger<DayItemService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }

    public async Task<GetMultipleDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var dayItems = await _database.RoosterItemDays
            .Include(x => x.LinkUserDayItems)
            .Where(x => x.CustomerId == customerId && x.DeletedOn == null
                && ((x.DateStart >= startDate && x.DateStart <= tillDate) || (x.DateEnd >= startDate && x.DateEnd <= tillDate))
                && (userId == Guid.Empty || x.LinkUserDayItems == null || x.LinkUserDayItems.Count == 0 || x.LinkUserDayItems.Any(x => x.UserId == userId)))
            .OrderBy(x => x.DateStart)
            .Select(x => x.ToRoosterItemDay())
            .ToListAsync(clt);
        var result = new GetMultipleDayItemResponse
        {
            DayItems = dayItems
        };
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetMultipleDayItemResponse> GetAllFutureDayItems(Guid customerId, int count, int skip, bool forAllUsers, Guid userId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var startDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        var dayItems = _database.RoosterItemDays
            .Include(x => x.LinkUserDayItems)
            .Where(x => x.CustomerId == customerId && x.DeletedOn == null
                && (forAllUsers || x.LinkUserDayItems!.Any(y => y.UserId == userId))
                && (x.DateStart >= startDate || x.DateEnd >= startDate))
            .OrderBy(x => x.DateStart)
            .Select(x => x.ToRoosterItemDay());
        var result = new GetMultipleDayItemResponse
        {
            DayItems = await dayItems.Skip(skip).Take(count).ToListAsync(clt),
            TotalCount = await dayItems.CountAsync(),
    };
        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        result.Success = true;
        return result;
    }

    public async Task<GetMultipleDayItemResponse> GetDayItemDashboard(Guid userId, Guid customerId, CancellationToken clt)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetMultipleDayItemResponse();

        var todayUtc = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);

        var dayItem = await _database.RoosterItemDays.Include(x => x.LinkUserDayItems)
            .Where(x => x.DeletedOn == null && x.CustomerId == customerId
            && (x.DateStart >= todayUtc || (x.DateEnd != null && x.DateEnd >= todayUtc))
            && ((x.LinkUserDayItems!.Any() && x.LinkUserDayItems!.Any(y => y.UserId == userId)) || (!x.LinkUserDayItems!.Any() && x.DateStart <= todayUtc.AddDays(7))))
            .OrderBy(x=>x.DateStart)
            .ToListAsync();
        if (dayItem is null)
        {
            result.Success = false;
        }
        else
        {
            result.DayItems = [];
            foreach (var item in dayItem)
            {
                result.DayItems.Add(item.ToRoosterItemDay());
            }
            result.Success = true;
        }

        sw.Stop();
        result.ElapsedMilliseconds = sw.ElapsedMilliseconds;
        return result;
    }

    public async Task<GetDayItemResponse> GetDayItemById(Guid customerId, Guid id, CancellationToken clt = default)
    {
        var sw = Stopwatch.StartNew();
        var result = new GetDayItemResponse();
        var dayItem = await _database.RoosterItemDays.Include(x => x.LinkUserDayItems).FirstOrDefaultAsync(x => x.Id == id && x.DeletedOn == null && x.CustomerId == customerId);
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
        if (roosterItemDay.LinkedUsers is not null)
        {
            foreach (var usr in roosterItemDay.LinkedUsers)
            {
                var dbLink = new DbLinkUserDayItem
                {
                    UserId = usr.UserId,
                    DayItemId = dbItem.Id
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
            if (roosterItemDay.LinkedUsers is not null)
            {
                foreach (var usr in roosterItemDay.LinkedUsers)
                {
                    if (dayItem.LinkUserDayItems?.Any(x => x.UserId == usr.UserId) is true)
                        continue;
                    var dbLink = new DbLinkUserDayItem
                    {
                        UserId = usr.UserId,
                        DayItemId = dayItem.Id
                    };
                    _database.LinkUserDayItems.Add(dbLink);
                }
                if (dayItem.LinkUserDayItems?.Any() is true)
                {
                    foreach (var usr in dayItem.LinkUserDayItems)
                    {
                        if (roosterItemDay.LinkedUsers.Any(x => x.UserId == usr.UserId))
                            continue;
                        _database.LinkUserDayItems.Remove(usr);
                    }
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

    public async Task<bool> PatchCalendarEventId(Guid dayItemId, Guid userId, Guid customerId, string? calendarEventId, CancellationToken clt)
    {
        var link = await _database.LinkUserDayItems.FirstOrDefaultAsync(x => x.DayItemId == dayItemId && x.UserId == userId);
        if (link is null) return false;
        link.CalendarEventId = calendarEventId;
        _database.LinkUserDayItems.Update(link);
        return (await _database.SaveChangesAsync(clt)) > 0;
    }
}
