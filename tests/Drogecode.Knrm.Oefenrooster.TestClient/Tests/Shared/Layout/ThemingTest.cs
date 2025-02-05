using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.TestPages.Shared.Layout;
using Microsoft.AspNetCore.Components.Web;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Shared.Layout;

public class ThemingTest : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void DefaultTheme([Frozen] IStringLocalizer<Theming> L1)
    {
        Localize(L1);
        var mudThemeProvider = RenderComponent<MudThemeProvider>().Instance;
        var cut = RenderComponent<ThemingTestPage>(parameters => parameters
            .Add(p => p.Global, new DrogeCodeGlobal())
            .Add(p => p.MudThemeProvider, mudThemeProvider));
        cut.Find("#dark-light-toggle").ParentElement!.TriggerEvent("onpointerenter", new PointerEventArgs());
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Switch to Light Theme"), TimeSpan.FromSeconds(2));
    }

    private void Localize(IStringLocalizer<Theming> L1)
    {
        Services.AddSingleton(L1);
        LocalizeA(L1, "Switch to Light Theme");
        LocalizeA(L1, "Switch to system");
        LocalizeA(L1, "Switch to Dark Theme");
    }
}