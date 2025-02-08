using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients.Interfaces;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;

public class MockConfigurationClient : IMockConfigurationClient
{
    private VersionDetailResponse _newVersionAvailable = new()
    {
        NewVersionAvailable = false,
        CurrentVersion = DefaultSettingsHelper.CURRENT_VERSION,
        UpdateVersion = DefaultSettingsHelper.UPDATE_VERSION,
        ButtonVersion = DefaultSettingsHelper.BUTTON_VERSION,
    };

    public void SetNewVersionAvailableResponse(VersionDetailResponse body)
    {
        _newVersionAvailable = body;
    }

    public async Task<VersionDetailResponse> NewVersionAvailableAsync(string clientVersion)
    {
        return _newVersionAvailable;
    }

    public async Task<UpgradeDatabaseResponse> UpgradeDatabaseAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UpgradeDatabaseResponse> UpgradeDatabaseAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<VersionDetailResponse> NewVersionAvailableAsync(string clientVersion, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<InstallingActiveResponse> InstallingActiveAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<InstallingActiveResponse> InstallingActiveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<UpdateSpecialDatesResponse> UpdateSpecialDatesAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<UpdateSpecialDatesResponse> UpdateSpecialDatesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<DbCorrectionResponse> DbCorrectionAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<DbCorrectionResponse> DbCorrectionAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}