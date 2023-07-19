using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class CalendarCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1)
    {
        Localize(L1);

        var training = new Training
        {
            Name = "xUnit meets bUnit",
            Availabilty = Shared.Enums.Availabilty.Available,
            DateStart = DateTime.UtcNow
        };
        var cut = RenderComponent<CalendarCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<App> L1)
    {
        Localize(L1);

        var training = new Training
        {
            Name = "xUnit meets bUnit",
            Availabilty = Shared.Enums.Availabilty.Available,
            DateStart = DateTime.UtcNow
        };
        var cut = RenderComponent<CalendarCard>(parameter => parameter
        .Add(p => p.Training, training));
        cut.Markup.Should().Contain("Available");
    }

    private void Localize(IStringLocalizer<App> L1)
    {
        Services.AddSingleton(L1);

        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
        A.CallTo(() => L1["Availibility"]).Returns(new LocalizedString("Availibility", "Availibility"));
        A.CallTo(() => L1["Available"]).Returns(new LocalizedString("Available", "Available"));
        A.CallTo(() => L1["NotAvailable"]).Returns(new LocalizedString("NotAvailable", "NotAvailable"));
        A.CallTo(() => L1["Maybe"]).Returns(new LocalizedString("Maybe", "Maybe"));
    }
}
