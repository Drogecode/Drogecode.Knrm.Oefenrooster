using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;

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
        var response = await _httpClient.GetFromJsonAsync<GetMonthItemResponse>($"api/CalendarItem/GetMonthItem?year={year}&month={month}");
        return response;
    }
}