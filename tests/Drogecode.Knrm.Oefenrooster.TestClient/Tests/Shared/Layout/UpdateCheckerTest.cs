using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients.Interfaces;
using Drogecode.Knrm.Oefenrooster.TestClient.TestPages.Shared.Layout;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Shared.Layout;

public class UpdateCheckerTest : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void NoUpdateAvailable([Frozen] IStringLocalizer<UpdateChecker> L1)
    {
        Localize(L1);
        var configurationClient = Services.GetService<IConfigurationClient>() as IMockConfigurationClient;
        var cut = RenderComponent<UpdateCheckerTestPage>();
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain("Update available"), TimeSpan.FromSeconds(2));
    }
    
    [Theory]
    [AutoFakeItEasyData]
    public void UpdateAvailable([Frozen] IStringLocalizer<UpdateChecker> L1)
    {
        Localize(L1);
        var configurationClient = Services.GetService<IConfigurationClient>() as IMockConfigurationClient;
        Assert.NotNull(configurationClient);
        configurationClient.SetNewVersionAvailableResponse(new VersionDetailResponse()
        {
            NewVersionAvailable = true,
            CurrentVersion = DefaultSettingsHelper.CURRENT_VERSION,
            UpdateVersion = DefaultSettingsHelper.UPDATE_VERSION + 5,
            ButtonVersion = DefaultSettingsHelper.BUTTON_VERSION + 5
        });
        var cut = RenderComponent<UpdateCheckerTestPage>();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Update available"), TimeSpan.FromSeconds(2));
    }
    
    [Theory]
    [AutoFakeItEasyData]
    public void OldButNoMessage([Frozen] IStringLocalizer<UpdateChecker> L1)
    {
        Localize(L1);
        var configurationClient = Services.GetService<IConfigurationClient>() as IMockConfigurationClient;
        Assert.NotNull(configurationClient);
        configurationClient.SetNewVersionAvailableResponse(new VersionDetailResponse()
        {
            NewVersionAvailable = true,
            CurrentVersion = DefaultSettingsHelper.CURRENT_VERSION,
            UpdateVersion = DefaultSettingsHelper.UPDATE_VERSION + 1,
            ButtonVersion = DefaultSettingsHelper.BUTTON_VERSION + 1
        });
        var cut = RenderComponent<UpdateCheckerTestPage>();
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain("Update available"), TimeSpan.FromSeconds(2));
    }

    private void Localize(IStringLocalizer<UpdateChecker> L1)
    {
        Services.AddSingleton(L1);

        A.CallTo(() => L1["Click to reload"]).Returns(new LocalizedString("Click to reload", "Click to reload"));
        A.CallTo(() => L1["Update available"]).Returns(new LocalizedString("Update available", "Update available"));
    }
}