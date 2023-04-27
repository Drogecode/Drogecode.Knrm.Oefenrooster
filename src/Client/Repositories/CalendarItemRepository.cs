using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class CalendarItemRepository
{
    private readonly HttpClient _httpClient;

    public CalendarItemRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GetMonthItemResponse?> GetMonthItemAsync(int year, int month, CancellationToken token)
    {
        var response = await _httpClient.GetFromJsonAsync<GetMonthItemResponse>($"api/CalendarItem/GetMonthItems?year={year}&month={month}");
        return response;
    }

    public async Task<GetDayItemResponse?> GetDayItemsAsync(DateRange dateRange, CancellationToken token)
    {
        DateTime? forMonth = PlannerHelper.ForMonth(dateRange);
        var response = await _httpClient.GetFromJsonAsync<GetDayItemResponse>($"api/CalendarItem/GetDayItems?forMonth={forMonth?.Month ?? 0}&yearStart={dateRange.Start!.Value.Year}&monthStart={dateRange.Start!.Value.Month}&dayStart={dateRange.Start!.Value.Day}&yearEnd={dateRange.End!.Value.Year}&monthEnd={dateRange.End!.Value.Month}&dayEnd={dateRange.End!.Value.Day}");
        return response;
    }
}