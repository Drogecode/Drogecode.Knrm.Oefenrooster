using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

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

    public async Task<GetMonthItemResponse> GetMonthItems(int year, int month, Guid customerId, CancellationToken token)
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
            }).ToListAsync(token);
        var result = new GetMonthItemResponse
        {
            MonthItems = monthItems,
        };
        return result;
    }

    public async Task<GetDayItemResponse> GetDayItems(int yearStart, int monthStart, int dayStart, int yearEnd, int monthEnd, int dayEnd, Guid customerId, CancellationToken token)
    {
        var startDate = (new DateTime(yearStart, monthStart, dayStart, 0, 0, 0)).ToUniversalTime();
        var tillDate = (new DateTime(yearEnd, monthEnd, dayEnd, 23, 59, 59, 999)).ToUniversalTime();
        var dayItems = await _database.RoosterItemDays.Where(x => x.CustomerId == customerId && x.DateStart >= startDate && x.DateStart <= tillDate).Select(x => new RoosterItemDay
        {
            Id = x.Id,
            Text = x.Text,
            DateStart = x.DateStart,
            IsFullDay = x.IsFullDay,
            Type = x.Type,
        }).ToListAsync(token);
        var result = new GetDayItemResponse
        {
            DayItems = dayItems
        };
        return result;
    }
}
