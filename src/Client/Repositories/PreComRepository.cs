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

    public async Task<MultiplePreComAlertsResponse?> GetAll(int take, int skip, CancellationToken clt)
    {
        var result = (await _preComClient.AllAlertsAsync(take, skip, clt));
        return result;
    }
}
