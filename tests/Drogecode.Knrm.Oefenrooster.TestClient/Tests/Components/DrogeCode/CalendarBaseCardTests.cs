using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class CalendarBaseCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L)
    {
        Localize(L);

        var training = new TrainingAdvance { Name = "xUnit meets bUnit" };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameButDayTest([Frozen] IStringLocalizer<App> L)
    {
        Localize(L);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.ReplaceEmtyName, true));
        cut.Markup.Should().Contain(training.DateStart.ToLocalTime().ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameNoReplaceTest([Frozen] IStringLocalizer<App> L)
    {
        Localize(L);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().NotContain(training.DateStart.ToLocalTime().ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickHistoryTest([Frozen] IStringLocalizer<App> L)
    {
        Localize(L);

        Assert.False(Clicked);
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickHistory, ClickButton));
        cut.Markup.Should().Contain(Icons.Material.Filled.History);
        cut.Markup.Should().NotContain(Icons.Material.Filled.Settings);
        var d = cut.Find(".DrogeCode-card-header-history");
        d.Click();
        Assert.True(Clicked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickSettingsTest([Frozen] IStringLocalizer<App> L)
    {
        Localize(L);

        Assert.False(Clicked);
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton));
        cut.Markup.Should().NotContain(Icons.Material.Filled.History);
        cut.Markup.Should().Contain(Icons.Material.Filled.Settings);
        var d = cut.Find(".DrogeCode-card-header-settings");
        d.Click();
        Assert.True(Clicked);
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickHistoryAndSettingsTest([Frozen] IStringLocalizer<App> L)
    {
        Localize(L);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton).Add(p => p.OnClickHistory, ClickButton));
        cut.Markup.Should().NotContain(Icons.Material.Filled.History);
        cut.Markup.Should().NotContain(Icons.Material.Filled.Settings);
        cut.Markup.Should().Contain(Icons.Material.Filled.MoreVert);
        var d = cut.Find(".DrogeCode-card-header-more-icons");
        d.Click();
        cut.Markup.Should().Contain(Icons.Material.Filled.History);
        cut.Markup.Should().Contain(Icons.Material.Filled.Settings);
        cut.Markup.Should().NotContain(Icons.Material.Filled.MoreVert);
    }

    private bool Clicked = false;
    private void ClickButton()
    {
        Clicked = true;
    }

    private void Localize(IStringLocalizer<App> L1)
    {
        Services.AddSingleton(L1);
        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
    }
}
