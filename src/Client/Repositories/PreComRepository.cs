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

    public async Task<List<PreComAlert>?> GetAll(CancellationToken clt)
    {
        var result = (await _preComClient.AllAlertsAsync(clt)).PreComAlerts;
        return result;
    }
}
