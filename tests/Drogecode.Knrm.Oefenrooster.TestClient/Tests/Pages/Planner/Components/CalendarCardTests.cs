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
    public void NotAvailableTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new Training
        {
            Name = "xUnit meets bUnit",
            Availability = Availability.NotAvailable,
            DateStart = DateTime.UtcNow
        };
        var cut = RenderComponent<CalendarCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("NotAvailable");
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<DateToString> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);

        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
        LocalizeA(L1, "Availability");
        LocalizeA(L1, "Available");
        LocalizeA(L1, "NotAvailable");
        LocalizeA(L1, "Maybe");
    }
}