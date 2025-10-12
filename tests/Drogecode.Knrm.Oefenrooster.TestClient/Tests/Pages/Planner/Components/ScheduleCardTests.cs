using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Pages.Planner.Components;

public class ScheduleCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleCard> L2, [Frozen] IStringLocalizer<DateToString> L3, [Frozen] IStringLocalizer<ReadMoreChip> L4, 
        [Frozen] IStringLocalizer<ReadReportChip> L5, [Frozen] IStringLocalizer<CalendarBaseCard> L6)
    {
        Localize(L1, L2, L3, L4, L5, L6);

        var training = new PlannedTraining
        {
            Name = "xUnit meets bUnit",
            ShowTime = true,
        };
        var cut = RenderComponent<ScheduleCard>(parameter => parameter
        .Add(p => p.Planner, training)
        .Add(p => p.Global, new DrogeCodeGlobal()));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }
    
    [Theory]
    [AutoFakeItEasyData]
    public void HasInformationTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<ScheduleCard> L2, [Frozen] IStringLocalizer<DateToString> L3, [Frozen] IStringLocalizer<ReadMoreChip> L4, 
        [Frozen] IStringLocalizer<ReadReportChip> L5, [Frozen] IStringLocalizer<CalendarBaseCard> L6)
    {
        Localize(L1, L2, L3, L4, L5, L6);

        var training = new PlannedTraining
        {
            Name = "xUnit meets bUnit",
            ShowTime = true,
            Description = "testje",
            HasDescription = true
        };
        var cut = RenderComponent<ScheduleCard>(parameter => parameter
        .Add(p => p.Planner, training)
        .Add(p => p.Global, new DrogeCodeGlobal()));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Information"), TimeSpan.FromSeconds(2));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void ListTest([Frozen] IStringLocalizer<App> L1, IStringLocalizer<ScheduleCard> L2, [Frozen] IStringLocalizer<DateToString> L3, [Frozen] IStringLocalizer<ReadMoreChip> L4, 
        [Frozen] IStringLocalizer<ReadReportChip> L5, [Frozen] IStringLocalizer<CalendarBaseCard> L6, [Frozen] IStringLocalizer<ReadMoreChip> L7)
    {
        Localize(L1, L2, L3, L4, L5, L6);

        var cut = RenderComponent<ScheduleCard>(parameter => parameter
        .Add(p => p.Planner, Training)
        .Add(p => p.Functions, Functions)
        .Add(p => p.Vehicles, Vehicles)
        .Add(p => p.Global, new DrogeCodeGlobal()));
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

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<ScheduleCard> L2, IStringLocalizer<DateToString> L3, IStringLocalizer<ReadMoreChip> L4, 
        IStringLocalizer<ReadReportChip> L5, IStringLocalizer<CalendarBaseCard> L6)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        Services.AddSingleton(L3);
        Services.AddSingleton(L4);
        Services.AddSingleton(L5);
        Services.AddSingleton(L6);

        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
        LocalizeA(L2, "Read more");
        LocalizeA(L4, "Information");
        LocalizeA(L6, "Settings");
        LocalizeA(L6, "History");
        LocalizeA(L6, "Rate");
    }
}
