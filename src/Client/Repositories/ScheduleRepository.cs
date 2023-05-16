using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ScheduleRepository
{
    private readonly IScheduleClient _scheduleClient;

    public ScheduleRepository(IScheduleClient scheduleClient)
    {
        _scheduleClient = scheduleClient;
    }
    public async Task<ScheduleForUserResponse?> CalendarForUser(DateRange dateRange, CancellationToken clt)
    {
        var schedule = await _scheduleClient.ForUserAsync(dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, clt);
        return schedule;
    }
    public async Task<ScheduleForAllResponse?> ScheduleForAll(DateRange dateRange, CancellationToken clt)
    {
        DateTime? forMonth = PlannerHelper.ForMonth(dateRange);
        var schedule = await _scheduleClient.ForAllAsync(forMonth?.Month ?? 0, dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, clt);
        return schedule;
    }

    public async Task<Training> PatchScheduleForUser(Training training, CancellationToken clt)
    {
        var result = await _scheduleClient.PatchScheduleForUserAsync(training, clt);
        return result;
    }
    public async Task<bool> PatchTraining(EditTraining patchedTraining, CancellationToken clt)
    {
        var result = await _scheduleClient.PatchTrainingAsync(patchedTraining, clt);
        return result;
    }
    public async Task<Guid> AddTraining(EditTraining newTraining, CancellationToken clt)
    {
        var result = await _scheduleClient.AddTrainingAsync(newTraining, clt);
        return result;
    }

    public async Task PatchAvailabilityUser(Guid? trainingId, PlanUser user, CancellationToken clt)
    {
        var body = new PatchAssignedUserRequest
        {
            TrainingId = trainingId,
            User = user,
        };
        await _scheduleClient.PatchAssignedUserAsync(body, clt);
    }
    public async Task<GetScheduledTrainingsForUserResponse?> GetScheduledTrainingsForUser(CancellationToken clt)
    {
        var schedule = await _scheduleClient.GetScheduledTrainingsForUserAsync(clt);
        return schedule;
    }

    public async Task<GetPinnedTrainingsForUserResponse?> GetPinnedTrainingsForUser(CancellationToken clt)
    {
        var schedule = await _scheduleClient.GetPinnedTrainingsForUserAsync(clt);
        return schedule;
    }

    internal async Task PutAssignedUser(bool assigned, Guid? trainingId, Guid functionId, DrogeUser user, CancellationToken clt)
    {
        var body = new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            FunctionId = functionId,
            UserId = user.Id,
            Assigned = assigned
        };
        await _scheduleClient.PutAssignedUserAsync(body, clt);
    }

    internal async Task<List<PlannerTrainingType>> GetTrainingTypes(CancellationToken clt = default)
    {
        var schedule = await _scheduleClient.GetTrainingTypesAsync(clt);
        return schedule.ToList();
    }
}
