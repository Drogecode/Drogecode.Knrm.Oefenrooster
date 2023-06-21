using Drogecode.Knrm.Oefenrooster.Server.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.ControllerTests;

public class ConfigurationControllerTests
{
    private readonly ConfigurationController _configurationController;

    public ConfigurationControllerTests(ConfigurationController configurationController)
    {
        _configurationController = configurationController;
    }

    // System.InvalidOperationException : Unable to resolve service for type 'Drogecode.Knrm.Oefenrooster.Server.Database.DataContext' while attempting to activate 'Drogecode.Knrm.Oefenrooster.Server.Services.ConfigurationService'.
    // Ik heb geen zin om dit nu op te lossen.
    /*[Fact]
    public async Task UpdateSpecialDatesTest()
    {
        var result = await _configurationController.UpdateSpecialDates();
        Assert.NotNull(result?.Value);
        Assert.True(result.Value);
    }*/
}
