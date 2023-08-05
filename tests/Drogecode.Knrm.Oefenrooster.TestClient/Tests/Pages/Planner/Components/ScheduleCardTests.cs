using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class ScheduleCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleCard> L2)
    {
        Localize(L1, L2);

        var training = new PlannedTraining { Name = "xUnit meets bUnit" };
        var cut = RenderComponent<ScheduleCard>(parameter => parameter.Add(p => p.Planner, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleCard> L2)
    {
        Localize(L1, L2);

        var cut = RenderComponent<ScheduleCard>(parameter => parameter
        .Add(p => p.Planner, Training)
        .Add(p => p.Functions, Functions));
        cut.Markup.Should().Contain("test user 1");
        cut.Markup.Should().NotContain("test user 2");
        cut.Markup.Should().NotContain("test user 3");
        cut.Markup.Should().NotContain("test user 4");
        cut.Markup.Should().Contain("test user 5");
        cut.Markup.Should().Contain("Test function 1");
        cut.Markup.Should().NotContain("Test function 2");
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<ScheduleCard> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);

        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
    }
}
