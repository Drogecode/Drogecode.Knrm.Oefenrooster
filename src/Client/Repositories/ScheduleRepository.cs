using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ScheduleRepository
{
    private readonly HttpClient _httpClient;

    public ScheduleRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ScheduleForUserResponse?> ScheduleForUser()
    {
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForUserResponse>("api/Schedule/ForUser");
        return schedule;
    }
}
