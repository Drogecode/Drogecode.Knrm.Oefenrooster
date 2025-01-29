using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class CalendarCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new Training
        {
            Name = "xUnit meets bUnit",
            Availability = Availability.Available,
            DateStart = DateTime.UtcNow,
            ShowTime = true,
        };
        var cut = RenderComponent<CalendarCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new Training
        {
            Name = "xUnit meets bUnit",
            Availability = Availability.Available,
            DateStart = DateTime.UtcNow
        };
        var cut = RenderComponent<CalendarCard>(parameter => parameter
            .Add(p => p.Training, training));
        cut.Markup.Should().Contain("Available");
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<DateToString> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);

        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
        A.CallTo(() => L1["Availibility"]).Returns(new LocalizedString("Availibility", "Availibility"));
        A.CallTo(() => L1["Available"]).Returns(new LocalizedString("Available", "Available"));
        A.CallTo(() => L1["NotAvailable"]).Returns(new LocalizedString("NotAvailable", "NotAvailable"));
        A.CallTo(() => L1["Maybe"]).Returns(new LocalizedString("Maybe", "Maybe"));
    }
}