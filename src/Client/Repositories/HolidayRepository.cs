using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class HolidayRepository
{
    private readonly IHolidayClient _holidayClient;
    private readonly IOfflineService _offlineService;

    public HolidayRepository(IHolidayClient holidayClient, IOfflineService offlineService)
    {
        _holidayClient = holidayClient;
        _offlineService = offlineService;
    }

    public async Task<Holiday?> Get(Guid id, CancellationToken clt)
    {
        var holiday = await _holidayClient.GetAsync(id, clt);
        return holiday?.Holiday;
    }

    public async Task<List<Holiday>?> GetAll(CancellationToken clt)
    {
        var result = await _offlineService.CachedRequestAsync("List_hol_usr",
            async () => await _holidayClient.GetAllAsync(clt),
            clt: clt);
        return result.Holidays;
    }

    public async Task<MultipleHolidaysResponse?> GetAllFuture(Guid? userId, bool cachedAndReplace, CancellationToken clt)
    {
        if (userId is null) return null;
        var schedule = await _offlineService.CachedRequestAsync(string.Format("AllFuHol_{0}", userId),
            async () => { return await _holidayClient.GetAllFutureAsync(30, cachedAndReplace, clt); },
            new ApiCachedRequest { CachedAndReplace = cachedAndReplace }, clt);
        return schedule;
    }

    public async Task<PutHolidaysForUserResponse?> PutHolidayForUser(Holiday body, CancellationToken clt = default)
    {
        var result = await _holidayClient.PutHolidayForUserAsync(body, clt);
        return result;
    }

    public async Task<PatchHolidaysForUserResponse?> PatchHolidayForUser(Holiday body, CancellationToken clt = default)
    {
        var result = await _holidayClient.PatchHolidayForUserAsync(body, clt);
        return result;
    }

    public async Task<DeleteResponse> Delete(Guid id, CancellationToken clt)
    {
        var result = await _holidayClient.DeleteAsync(id, clt);
        return result;
    }
}
