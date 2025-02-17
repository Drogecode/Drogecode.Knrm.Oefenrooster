using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class CalendarBaseCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new TrainingAdvance
        {
            Name = "xUnit meets bUnit",
            ShowTime = true,
        };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameButDayTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.ReplaceEmptyName, true));
        var timeZone = "Europe/Amsterdam";
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var dateStart = TimeZoneInfo.ConvertTimeFromUtc(training.DateStart, zone);
        cut.Markup.Should().Contain(dateStart.ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameNoReplaceTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().NotContain(training.DateStart.ToLocalTime().ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickHistoryTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        Assert.False(Clicked);
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickHistory, ClickButton));
        cut.Markup.Should().Contain(Icons.Material.Filled.History);
        cut.Markup.Should().NotContain(Icons.Material.Filled.Settings);
        var d = cut.Find(".DrogeCode-card-header-history");
        d.Click();
        cut.WaitForAssertion(() => Clicked.Should().BeTrue(), TimeSpan.FromSeconds(2));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickSettingsTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        Assert.False(Clicked);
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton));
        cut.Markup.Should().NotContain(Icons.Material.Filled.History);
        cut.Markup.Should().Contain(Icons.Material.Filled.Settings);
        var d = cut.Find(".DrogeCode-card-header-settings");
        d.Click();
        cut.WaitForAssertion(() => Clicked.Should().BeTrue(), TimeSpan.FromSeconds(2));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickHistoryAndSettingsTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2)
    {
        Localize(L1, L2);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton).Add(p => p.OnClickHistory, ClickButton));
        cut.Markup.Should().NotContain(Icons.Material.Filled.History);
        cut.Markup.Should().NotContain(Icons.Material.Filled.Settings);
        cut.Markup.Should().Contain(Icons.Material.Filled.MoreVert);
        var d = cut.Find(".DrogeCode-card-header-more-icons");
        d.Click();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.History), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.Settings), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain(Icons.Material.Filled.MoreVert), TimeSpan.FromSeconds(2));
    }

    private bool Clicked = false;

    private void ClickButton()
    {
        Clicked = true;
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<DateToString> L2)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
    }
}