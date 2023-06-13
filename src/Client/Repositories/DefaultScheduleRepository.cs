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

    public async Task<List<DefaultSchedule>?> GetAll(CancellationToken clt)
    {
        var result = await _offlineService.CachedRequestAsync("List_def_sche",
            async () => await _defaultScheduleClient.GetAllAsync(clt),
            clt: clt);
        return result.DefaultSchedules;
    }

    public async Task<DefaultSchedule?> PatchDefaultScheduleForUser(DefaultSchedule body, CancellationToken clt)
    {
        var result = await _defaultScheduleClient.PatchDefaultScheduleForUserAsync(body, clt);
        return result.Patched;
    }
}
