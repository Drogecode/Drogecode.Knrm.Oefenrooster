using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class CalendarItemRepository
{
    private readonly ICalendarItemClient _calendarItemClient;

    public CalendarItemRepository(ICalendarItemClient calendarItemClient)
    {
        _calendarItemClient = calendarItemClient;
    }

    public async Task<GetMonthItemResponse?> GetMonthItemAsync(int year, int month, CancellationToken clt)
    {
        var response = await _calendarItemClient.GetMonthItemsAsync(year, month, clt);
        return response;

    }

    public async Task<GetDayItemResponse?> GetDayItemsAsync(DateRange dateRange, CancellationToken clt)
    {
        var response = await _calendarItemClient.GetDayItemsAsync(dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, clt);
        return response;
    }
}