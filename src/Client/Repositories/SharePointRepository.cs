using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class SharePointRepository
{
    private readonly ISharePointClient _sharePointClient;

    public SharePointRepository(ISharePointClient sharePointClient)
    {
        _sharePointClient = sharePointClient;
    }

    public async Task<List<SharePointTraining>> GetLastTrainingsForCurrentUser(CancellationToken clt)
    {
       return (await _sharePointClient.GetLastTrainingsForCurrentUserAsync(5, clt)).ToList();
    }
}
