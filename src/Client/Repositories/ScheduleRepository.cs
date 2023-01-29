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
    public async Task<ScheduleForUserResponse?> CalendarForUser(int relativeWeek, CancellationToken token)
    {
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForUserResponse>($"api/Schedule/ForUser?relativeWeek={relativeWeek}", token);
        return schedule;
    }
    public async Task<ScheduleForAllResponse?> ScheduleForAll(int relativeWeek, CancellationToken token)
    {
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForAllResponse>($"api/Schedule/ForAll?relativeWeek={relativeWeek}", token);
        return schedule;
    }
    public async Task<Training> PatchScheduleForUser(Training training, CancellationToken token)
    {
        var request = await _httpClient.PostAsJsonAsync<Training>($"api/Schedule/Patch", training, token);
        var result = await request.Content.ReadFromJsonAsync<Training>(cancellationToken: token);

        return result;
    }

    public async Task PatchScheduleUserScheduled(Guid? trainingId, PlanUser user, CancellationToken token)
    {
        var body = new PatchScheduleUserRequest
        {
            TrainingId = trainingId,
            User = user,
        };
        var request = await _httpClient.PostAsJsonAsync<PatchScheduleUserRequest>($"api/Schedule/PatchScheduleUser", body, token);
    }
    public async Task<GetScheduledTrainingsForUserResponse?> GetScheduledTrainingsForUser(CancellationToken token)
    {
        var schedule = await _httpClient.GetFromJsonAsync<GetScheduledTrainingsForUserResponse>($"api/Schedule/GetScheduledTrainingsForUser", token);
        return schedule;
    }
}
