using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class MonthItemRepository
{
    private readonly IMonthItemClient _monthItemClient;
    private readonly IOfflineService _offlineService;

    private const string MONTHITEMS = "Mon_it_{0}_{1}";

    public MonthItemRepository(IMonthItemClient monthItemClient, IOfflineService offlineService)
    {
        _monthItemClient = monthItemClient;
        _offlineService = offlineService;
    }

    public async Task<GetMultipleMonthItemResponse?> GetMonthItemAsync(int year, int month, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(MONTHITEMS, year, month),
            async () => await _monthItemClient.GetItemsAsync(year, month, clt),
            clt: clt);
        return response;
    }
}
