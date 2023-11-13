using Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

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

    public async Task<MultipleSharePointTrainingsResponse?> GetLastTrainingsForCurrentUser(int count, int skip, CancellationToken clt)
    {
        var result = await _sharePointClient.GetLastTrainingsForCurrentUserAsync(count, skip, clt);
        return result;
    }

    public async Task<MultipleSharePointActionsResponse?> GetLastActionsForCurrentUser(int count, int skip, CancellationToken clt)
    {
        var result = await _sharePointClient.GetLastActionsForCurrentUserAsync(count, skip, clt);
        return result;
    }

    public async Task<MultipleSharePointActionsResponse> GetLastActions(IEnumerable<DrogeUser> users, int count, int skip, CancellationToken clt)
    {
        var workingList = new List<Guid>();
        foreach(var user in users)
        {
            workingList.Add(user.Id);
        }
        var usersAsstring = System.Text.Json.JsonSerializer.Serialize(workingList);
        var result = await _sharePointClient.GetLastActionsAsync(usersAsstring, count, skip, clt);
        return result;
    }

    public async Task<MultipleSharePointTrainingsResponse?> GetLastTrainings(IEnumerable<DrogeUser> users, int count, int skip, CancellationToken clt)
    {
        var workingList = new List<Guid>();
        foreach(var user in users)
        {
            workingList.Add(user.Id);
        }
        var usersAsstring = System.Text.Json.JsonSerializer.Serialize(workingList);
        var result = await _sharePointClient.GetLastTrainingsAsync(usersAsstring, count, skip, clt);
        return result;
    }
}
