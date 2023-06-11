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

    public async Task<List<SharePointTraining>?> GetLastTrainingsForCurrentUser(CancellationToken clt)
    {
        var result = await _sharePointClient.GetLastTrainingsForCurrentUserAsync(5, clt);
        return result.SharePointTrainings?.ToList();
    }

    public async Task<List<SharePointAction>?> GetLastActionsForCurrentUser(CancellationToken clt)
    {
        var result = await _sharePointClient.GetLastActionsForCurrentUserAsync(5, clt);
        return result.SharePointActions?.ToList();
    }
}
