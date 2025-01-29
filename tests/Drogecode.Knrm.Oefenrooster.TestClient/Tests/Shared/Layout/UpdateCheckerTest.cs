using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Shared.Layout;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Shared.Layout;

public class UpdateCheckerTest : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<UpdateChecker> L1)
    {
        Localize(L1);

        var cut = RenderComponent<UpdateChecker>();
        cut.Markup.Should().Contain("test user 1");
        cut.Markup.Should().NotContain("test user 2");
        cut.Markup.Should().NotContain("test user 3");
        cut.Markup.Should().NotContain("test user 4");
        cut.Markup.Should().Contain("test user 5");
        cut.Markup.Should().Contain("Test function 1");
        cut.Markup.Should().NotContain("Test function 2");
        cut.Markup.Should().Contain("Vehicle 1 default");
        cut.Markup.Should().Contain("Vehicle 2 not default");
        cut.Markup.Should().NotContain("Vehicle 3 not selected");
    }

    private void Localize(IStringLocalizer<UpdateChecker> L1)
    {
        Services.AddSingleton(L1);

        A.CallTo(() => L1["Click to reload"]).Returns(new LocalizedString("Click to reload", "Click to reload"));
        A.CallTo(() => L1["Update available"]).Returns(new LocalizedString("Update available", "Update available"));
    }
}