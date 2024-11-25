using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class PreComRepository
{
    private readonly IPreComClient _preComClient;

    public PreComRepository(IPreComClient preComClient)
    {
        _preComClient = preComClient;
    }

    public async Task<MultiplePreComAlertsResponse?> GetAllAlerts(int take, int skip, CancellationToken clt)
    {
        var result = (await _preComClient.AllAlertsAsync(take, skip, clt));
        return result;
    }

    public async Task<MultiplePreComForwardsResponse?> GetAllForwards(int take, int skip, CancellationToken clt)
    {
        var result = (await _preComClient.AllForwardsAsync(take, skip, clt));
        return result;
    }

    public async Task<MultiplePreComForwardsResponse?> AllForwardsForUserAsync(Guid userId, int take, int skip, CancellationToken clt)
    {
        var result = (await _preComClient.AllForwardsForUserAsync(userId, take, skip, clt));
        return result;
    }
    public async Task<bool> PostForwardAsync(PostForwardRequest body, CancellationToken clt)
    {
        var result = await _preComClient.PostForwardAsync(body, clt);
        return result;
    }
}
