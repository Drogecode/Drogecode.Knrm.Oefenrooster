using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Server.Services;

public class CalendarItemService : ICalendarItemService
{
    private readonly ILogger<CalendarItemService> _logger;
    private readonly Database.DataContext _database;
    public CalendarItemService(ILogger<CalendarItemService> logger, Database.DataContext database)
    {
        _logger = logger;
        _database = database;
    }
    public async Task<GetMonthItemResponse> GetMonthItem(int year, int month, Guid customerId)
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
            }).ToListAsync();
        var result = new GetMonthItemResponse
        {
            MonthItems = monthItems,
        };
        return result;
    }
}
