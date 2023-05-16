﻿using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
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
        return response;
    }

    public async Task<UpdateDetails?> NewVersionAvailable()
    {
        var clientVersion = DefaultSettingsHelper.CURRENT_VERSION;
        var response = await _configurationClient.NewVersionAvailableAsync(clientVersion);
        return response;
    }

    public async Task<bool?> InstallingActive()
    {
        var response = await _configurationClient.InstallingActiveAsync();
        return response;
    }

    public async Task<bool?> UpdateSpecialDates()
    {
        var response = await _configurationClient.UpdateSpecialDatesAsync();
        return response;
    }
}
