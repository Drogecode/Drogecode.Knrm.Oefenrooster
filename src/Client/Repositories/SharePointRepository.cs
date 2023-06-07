using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class SharePointRepository
{
    private readonly ISharePointClient _sharePointClient;

    public SharePointRepository(ISharePointClient sharePointClient)
    {
        _sharePointClient = sharePointClient;
    }

    public async Task GetLastTrainingsForCurrentUser(CancellationToken clt)
    {
        await _sharePointClient.GetLastTrainingsForCurrentUserAsync(5, clt);
    }
}
