using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using System.Net.Http.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ScheduleRepository
{
    private readonly HttpClient _httpClient;

    public ScheduleRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ScheduleForUserResponse?> CalendarForUser(int relativeWeek)
    {
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForUserResponse>($"api/Schedule/ForUser?relativeWeek={relativeWeek}");
        return schedule;
    }
    public async Task<ScheduleForAllResponse?> ScheduleForAll(int relativeWeek)
    {
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForAllResponse>($"api/Schedule/ForAll?relativeWeek={relativeWeek}");
        return schedule;
    }
    public async Task<Training> PatchScheduleForUser(Training training)
    {
        var request = await _httpClient.PostAsJsonAsync<Training>($"api/Schedule/Patch", training);
        var result = await request.Content.ReadFromJsonAsync<Training>();

        return result;
    }
}
