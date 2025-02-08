using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients.Interfaces;

public interface IMockConfigurationClient : IConfigurationClient
{
    public void SetNewVersionAvailableResponse(VersionDetailResponse body);
}