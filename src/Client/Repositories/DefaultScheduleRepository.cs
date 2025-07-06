using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class DefaultScheduleRepository
{
    private readonly IDefaultScheduleClient _defaultScheduleClient;
    private readonly IOfflineService _offlineService;

    public DefaultScheduleRepository(IDefaultScheduleClient defaultScheduleClient, IOfflineService offlineService)
    {
        _defaultScheduleClient = defaultScheduleClient;
        _offlineService = offlineService;
    }

    public async Task<List<DefaultGroup>?> GetAllGroups(CancellationToken clt)
    {
        var result = await _offlineService.CachedRequestAsync("List_def_group",
            async () => await _defaultScheduleClient.GetAllGroupsAsync(clt),
            clt: clt);
        return result.Groups;
    }

    public async Task<List<DefaultSchedule>?> GetAllByGroupId(Guid groupId, CancellationToken clt)
    {
        var result = await _defaultScheduleClient.GetAllByGroupIdAsync(groupId, clt);
        if (result?.DefaultSchedules is null)
            return null;
        foreach (var schedule in result.DefaultSchedules.Where(schedule => schedule.UserSchedules is null || schedule.UserSchedules.Count == 0))
        {
            schedule.UserSchedules = [new DefaultUserSchedule()];
        }

        return result.DefaultSchedules;
        // Do not use cache for this one
        /*var result = await _offlineService.CachedRequestAsync(string.Format("List_def_sche_{0}", groupId),
            async () => await _defaultScheduleClient.GetAllByGroupIdAsync(groupId, clt),
             clt: clt);
        return result.DefaultSchedules;*/
    }

    public async Task<PatchDefaultUserSchedule?> PatchDefaultScheduleForUser(PatchDefaultUserSchedule body, CancellationToken clt)
    {
        var result = await _defaultScheduleClient.PatchDefaultScheduleForUserAsync(body, clt);
        return result.Patched;
    }

    public async Task<PatchDefaultScheduleResponse?> PatchDefaultScheduleAsync(DefaultSchedule? body, CancellationToken clt)
    {
        var result = await _defaultScheduleClient.PatchDefaultScheduleAsync(body, clt);
        return result;
    }

    public async Task<PutGroupResponse> PutGroup(DefaultGroup body, CancellationToken clt)
    {
        var result = await _defaultScheduleClient.PutGroupAsync(body, clt);
        return result;
    }

    public async Task<PutDefaultScheduleResponse> PutDefaultScheduleAsync(DefaultSchedule body, CancellationToken clt)
    {
        var result = await _defaultScheduleClient.PutDefaultScheduleAsync(body, clt);
        return result;
    }

    public async Task<GetAllDefaultScheduleResponse> GetAllDefaultSchedule(CancellationToken clt)
    {
        var result = await _defaultScheduleClient.GetAllDefaultScheduleAsync(clt);
        return result;
    }
}