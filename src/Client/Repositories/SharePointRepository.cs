using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class SharePointRepository
{
    private readonly ISharePointClient _sharePointClient;
    private readonly IOfflineService _offlineService;

    public SharePointRepository(ISharePointClient sharePointClient, IOfflineService offlineService)
    {
        _sharePointClient = sharePointClient;
        _offlineService = offlineService;
    }

    public async Task<List<SharePointTraining>?> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt)
    {
        var result = await _sharePointClient.GetLastTrainingsForCurrentUserAsync(count, skip, clt);
        return result.SharePointTrainings?.ToList();
    }

    public async Task<List<SharePointAction>?> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt)
    {
        var result = await _sharePointClient.GetLastActionsForCurrentUserAsync(count, skip, clt);
        return result.SharePointActions?.ToList();
    }
}
