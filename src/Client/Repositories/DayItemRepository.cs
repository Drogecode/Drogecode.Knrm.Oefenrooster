using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class DayItemRepository
{
    private readonly IDayItemClient _dayItemClient;
    private readonly IOfflineService _offlineService;

    private const string DAYITEMS = "Day_it_{0}_{1}_{2}";
    private const string DAYITEMSDASHBOARD = "Day_it_da_{0}";

    public DayItemRepository(IDayItemClient dayItemClient, IOfflineService offlineService)
    {
        _dayItemClient = dayItemClient;
        _offlineService = offlineService;
    }

    public async Task<GetMultipleDayItemResponse?> GetDayItemsAsync(DateRange dateRange, Guid userId, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(DAYITEMS, dateRange.Start?.ToString("yyMMdd"), dateRange.End?.ToString("yyMMdd"), userId),
            async () => await _dayItemClient.GetItemsAsync(dateRange.Start!.Value.Year, dateRange.Start!.Value.Month, dateRange.Start!.Value.Day, dateRange.End!.Value.Year, dateRange.End!.Value.Month, dateRange.End!.Value.Day, userId, clt),
            clt: clt);
        return response;
    }

    public async Task<GetMultipleDayItemResponse?> GetDayItemDashboardAsync(Guid? userId, bool cachedAndReplace, CancellationToken clt)
    {
        var response = await _offlineService.CachedRequestAsync(string.Format(DAYITEMSDASHBOARD, userId),
            async () => await _dayItemClient.GetDashboardAsync(cachedAndReplace, clt),
           new ApiCachedRequest{CachedAndReplace = cachedAndReplace}, clt);
        return response;
    }
}