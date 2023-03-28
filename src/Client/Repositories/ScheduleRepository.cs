using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Graph.Models;
using MudBlazor;
using System.Net.Http.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ScheduleRepository
{
    private readonly HttpClient _httpClient;

    public ScheduleRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<ScheduleForUserResponse?> CalendarForUser(DateRange dateRange, CancellationToken token)
    {
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForUserResponse>($"api/Schedule/ForUser?yearStart={dateRange.Start!.Value.Year}&monthStart={dateRange.Start!.Value.Month}&dayStart={dateRange.Start!.Value.Day}&yearEnd={dateRange.End!.Value.Year}&monthEnd={dateRange.End!.Value.Month}&dayEnd={dateRange.End!.Value.Day}", token);
        return schedule;
    }
    public async Task<ScheduleForAllResponse?> ScheduleForAll(DateRange dateRange, CancellationToken token)
    {
        int forMonth = 0;
        var findMonth = dateRange.Start!.Value;
        while (forMonth == 0)
        {
            Console.WriteLine(findMonth);
            if (findMonth.Day != 1)
            {
                if (findMonth.Month == dateRange.End!.Value.Month)
                    break;
                findMonth = findMonth.AddDays(1);
                continue;
            }
            forMonth = findMonth.Month;
            break;
        }
        var schedule = await _httpClient.GetFromJsonAsync<ScheduleForAllResponse>($"api/Schedule/ForAll?forMonth={forMonth}&yearStart={dateRange.Start!.Value.Year}&monthStart={dateRange.Start!.Value.Month}&dayStart={dateRange.Start!.Value.Day}&yearEnd={dateRange.End!.Value.Year}&monthEnd={dateRange.End!.Value.Month}&dayEnd={dateRange.End!.Value.Day}", token);
        return schedule;
    }
    public async Task<Training> PatchScheduleForUser(Training training, CancellationToken token)
    {
        var request = await _httpClient.PostAsJsonAsync($"api/Schedule/PatchScheduleForUser", training, token);
        var result = await request.Content.ReadFromJsonAsync<Training>(cancellationToken: token);

        return result;
    }
    public async Task<bool> PatchTraining(EditTraining patchedTraining, CancellationToken token)
    {
        var request = await _httpClient.PostAsJsonAsync($"api/Schedule/PatchTraining", patchedTraining, token);
        var result = await request.Content.ReadFromJsonAsync<bool>(cancellationToken: token);

        return result;
    }
    public async Task<Guid> AddTraining(EditTraining newTraining, CancellationToken token)
    {
        var request = await _httpClient.PostAsJsonAsync($"api/Schedule/AddTraining", newTraining, token);
        var result = await request.Content.ReadFromJsonAsync<Guid>(cancellationToken: token);

        return result;
    }

    public async Task PatchAvailabilityUser(Guid? trainingId, PlanUser user, CancellationToken token)
    {
        var body = new PatchScheduleUserRequest
        {
            TrainingId = trainingId,
            User = user,
        };
        var request = await _httpClient.PostAsJsonAsync($"api/Schedule/PatchAvailabilityUser", body, token);
        request.EnsureSuccessStatusCode();
    }
    public async Task<GetScheduledTrainingsForUserResponse?> GetScheduledTrainingsForUser(CancellationToken token)
    {
        var schedule = await _httpClient.GetFromJsonAsync<GetScheduledTrainingsForUserResponse>($"api/Schedule/GetScheduledTrainingsForUser", token);
        return schedule;
    }

    public async Task<GetPinnedTrainingsForUserResponse?> GetPinnedTrainingsForUser(CancellationToken token)
    {
        var schedule = await _httpClient.GetFromJsonAsync<GetPinnedTrainingsForUserResponse>($"api/Schedule/GetPinnedTrainingsForUser", token);
        return schedule;
    }

    internal async Task OtherScheduleUser(bool assigned, Guid? trainingId, Guid functionId, DrogeUser user, CancellationToken token)
    {
        var body = new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            FunctionId = functionId,
            UserId = user.Id,
            Assigned = assigned
        };
        var request = await _httpClient.PostAsJsonAsync($"api/Schedule/OtherScheduleUser", body, token);
        request.EnsureSuccessStatusCode();
    }

    internal async Task<List<PlannerTrainingType>> GetTrainingTypes(CancellationToken token = default)
    {
        var schedule = await _httpClient.GetFromJsonAsync<List<PlannerTrainingType>>($"api/Schedule/GetTrainingTypes", token);
        return schedule;
    }
}
