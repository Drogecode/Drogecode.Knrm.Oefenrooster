using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class CalendarItemRepository
{
    private readonly ICalendarItemClient _calendarItemClient;
    private readonly IOfflineService _offlineService;

    private const string MONTHITEMS = "Mon_it_{0}_{1}";
    private const string DAYITEMS = "Day_it_{0}_{1}";

    public CalendarItemRepository(ICalendarItemClient calendarItemClient, IOfflineService offlineService)
    {
        _calendarItemClient = calendarItemClient;
        _offlineService = offlineService;
    }

    public async Task<GetMonthItemResponse?> GetMonthItemAsync(int year, int month, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(MONTHITEMS, year, month),
            async () => await _calendarItemClient.GetMonthItemsAsync(year, month, clt),
            clt: clt);
        return response;
    }

    public async Task<GetDayItemResponse?> GetDayItemsAsync(DateRange dateRange, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(DAYITEMS, dateRange.Start?.ToString("yyMMdd"), dateRange.End?.ToString("yyMMdd")),
            async () => await _calendarItemClient.GetDayItemsAsync(dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, clt),
            clt: clt);
        return response;
    }
}