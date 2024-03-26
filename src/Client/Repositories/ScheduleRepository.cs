using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using MudBlazor.Extensions;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ScheduleRepository
{
    private readonly IScheduleClient _scheduleClient;
    private readonly IOfflineService _offlineService;

    public ScheduleRepository(IScheduleClient scheduleClient, IOfflineService offlineService)
    {
        _scheduleClient = scheduleClient;
        _offlineService = offlineService;
    }
    public async Task<MultipleTrainingsResponse?> CalendarForUser(DateRange dateRange, CancellationToken clt)
    {
        var schedule = await _scheduleClient.ForUserAsync(dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, clt);
        return schedule;
    }
    public async Task<ScheduleForAllResponse?> ScheduleForAll(DateRange dateRange, bool includeUnAssigned, CancellationToken clt)
    {
        DateTime? forMonth = PlannerHelper.ForMonth(dateRange);
        var schedule = await _scheduleClient.ForAllAsync(forMonth?.Month ?? 0, dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, includeUnAssigned, clt);
        return schedule;
    }

    public async Task<Training?> PatchScheduleForUser(Training training, CancellationToken clt)
    {
        var result = await _scheduleClient.PatchScheduleForUserAsync(training, clt);
        return result.PatchedTraining;
    }
    public async Task<bool> PatchTraining(PlannedTraining patchedTraining, CancellationToken clt)
    {
        var result = await _scheduleClient.PatchTrainingAsync(patchedTraining, clt);
        return result.Success;
    }
    public async Task<Guid> AddTraining(PlannedTraining newTraining, CancellationToken clt)
    {
        var result = await _scheduleClient.AddTrainingAsync(newTraining, clt);
        return result.NewId;
    }

    public async Task<PatchAssignedUserResponse> PatchAssignedUser(Guid? trainingId, TrainingAdvance? training, PlanUser user)
    {
        var body = new PatchAssignedUserRequest
        {
            TrainingId = trainingId,
            User = user,
            Training = training
        };
        return await _scheduleClient.PatchAssignedUserAsync(body);
    }

    public async Task<GetPlannedTrainingResponse?> GetPlannedTrainingById(Guid? trainingId, CancellationToken clt)
    {
        if (trainingId is null) return null;
        var schedule = await _offlineService.CachedRequestAsync(string.Format("plTr_{0}", trainingId),
            async () => { return await _scheduleClient.GetPlannedTrainingByIdAsync(trainingId.Value, clt); },
            clt: clt);
        return schedule;
    }

    public async Task<GetPlannedTrainingResponse?> GetPlannedTrainingForDefaultDate(DateTime date, Guid? defaultId, CancellationToken clt)
    {
        var schedule = await _offlineService.CachedRequestAsync(string.Format("plDef_{0}_{1}", defaultId, date.ToIsoDateString()),
            async () => { return await _scheduleClient.GetPlannedTrainingForDefaultDateAsync(date, defaultId, clt); },
            clt: clt);
        return schedule;
    }

    public async Task<GetScheduledTrainingsForUserResponse?> GetScheduledTrainingsForUser(Guid? userId, bool cachedAndReplace, CancellationToken clt)
    {
        if (userId is null) return null;
        var schedule = await _offlineService.CachedRequestAsync(string.Format("trFoUse_{0}", userId),
            async () => { return await _scheduleClient.GetScheduledTrainingsForUserAsync(cachedAndReplace, clt); },
            new ApiCachedRequest { CachedAndReplace = cachedAndReplace }, clt);
        return schedule;
    }

    public async Task<GetScheduledTrainingsForUserResponse?> AllTrainingsForUser(Guid? userId, CancellationToken clt)
    {
        if (userId is null) return null;
        var schedule = await _offlineService.CachedRequestAsync(string.Format("AlltrFoUse_{0}", userId),
            async () => { return await _scheduleClient.AllTrainingsForUserAsync(userId.Value, clt); },
            clt: clt);
        return schedule;
    }

    public async Task<GetPinnedTrainingsForUserResponse?> GetPinnedTrainingsForUser(CancellationToken clt)
    {
        var schedule = await _scheduleClient.GetPinnedTrainingsForUserAsync(clt);
        return schedule;
    }

    public async Task<bool> DeleteTraining(Guid? id, CancellationToken clt)
    {
        if (id is null) return false;
        var schedule = await _scheduleClient.DeleteTrainingAsync(id.Value, clt);
        return schedule;
    }

    internal async Task<PutAssignedUserResponse> PutAssignedUser(bool assigned, Guid? trainingId, Guid functionId, DrogeUser user, TrainingAdvance? training)
    {
        var body = new OtherScheduleUserRequest
        {
            TrainingId = trainingId,
            FunctionId = functionId,
            UserId = user.Id,
            Assigned = assigned,
            Training = training
        };
        return await _scheduleClient.PutAssignedUserAsync(body);
    }
}
