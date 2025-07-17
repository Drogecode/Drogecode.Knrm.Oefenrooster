using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;

namespace Drogecode.Knrm.Oefenrooster.Client.Repositories;

public class ConfigurationRepository
{
    private readonly IConfigurationClient _configurationClient;

    public ConfigurationRepository(IConfigurationClient configurationClient)
    {
        _configurationClient = configurationClient;
    }

    public async Task<bool> UpgradeDatabaseAsync()
    {
        var response = await _configurationClient.UpgradeDatabaseAsync();
        return response.Success;
    }

    public async Task<VersionDetailResponse?> NewVersionAvailable()
    {
        try
        {
            var clientVersion = DefaultSettingsHelper.CURRENT_VERSION;
            var response = await _configurationClient.NewVersionAvailableAsync(clientVersion);
            return response;
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
            return null;
        }
    }

    public async Task<bool?> UpdateSpecialDates()
    {
        var response = await _configurationClient.UpdateSpecialDatesAsync();
        return response.Success;
    }
    public async Task<DbCorrectionResponse?> DbCorrection(CancellationToken clt)
    {
        var response = await _configurationClient.DbCorrectionAsync(clt);
        return response;
    }

    public async Task<GetPerformanceSettingResponse> GetPerformanceSettingAsync(CancellationToken clt)
    {
        var response = await _configurationClient.GetPerformanceSettingAsync(clt);
        return response;
    }

    public async Task<BaseResponse> PatchPerformanceSettingAsync(PatchPerformanceSettingRequest body, CancellationToken clt)
    {
        var response = await _configurationClient.PatchPerformanceSettingAsync(body, clt);
        return response;
    }
}
