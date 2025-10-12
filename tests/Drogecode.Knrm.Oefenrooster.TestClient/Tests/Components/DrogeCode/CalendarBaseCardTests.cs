using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using Drogecode.Knrm.Oefenrooster.TestClient.TestPages.Components.DrogeCode;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class CalendarBaseCardTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2, [Frozen] IStringLocalizer<CalendarBaseCard> L3)
    {
        Localize(L1, L2, L3);

        var training = new TrainingAdvance
        {
            Name = "xUnit meets bUnit",
            ShowTime = true,
        };
        var cut = RenderComponent<CalendarBaseCardTestPage>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameButDayTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2, [Frozen] IStringLocalizer<CalendarBaseCard> L3)
    {
        Localize(L1, L2, L3);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCardTestPage>(parameter => parameter.Add(p => p.Training, training).Add(p => p.ReplaceEmptyName, true));
        var timeZone = "Europe/Amsterdam";
        var zone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var dateStart = TimeZoneInfo.ConvertTimeFromUtc(training.DateStart, zone);
        cut.Markup.Should().Contain(dateStart.ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameNoReplaceTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2, [Frozen] IStringLocalizer<CalendarBaseCard> L3)
    {
        Localize(L1, L2, L3);

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCardTestPage>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().NotContain(training.DateStart.ToLocalTime().ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task OnClickHistoryTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2, [Frozen] IStringLocalizer<CalendarBaseCard> L3)
    {
        Localize(L1, L2, L3);

        Assert.False(Clicked);
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCardTestPage>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickHistory, ClickButton));
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain(Icons.Material.Filled.Settings), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.History), TimeSpan.FromSeconds(2));
        await cut.Find(".DrogeCode-card-header-history").ClickAsync(new MouseEventArgs());
        cut.WaitForAssertion(() => Clicked.Should().BeTrue(), TimeSpan.FromSeconds(2));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task OnClickSettingsTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2, [Frozen] IStringLocalizer<CalendarBaseCard> L3)
    {
        Localize(L1, L2, L3);

        Assert.False(Clicked);
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCardTestPage>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton));
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain(Icons.Material.Filled.History), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.Settings));
        await cut.Find(".DrogeCode-card-header-settings").ClickAsync(new MouseEventArgs());
        cut.WaitForAssertion(() => Clicked.Should().BeTrue(), TimeSpan.FromSeconds(2));
    }

    [Theory]
    [AutoFakeItEasyData]
    public async Task OnClickHistoryAndSettingsTest([Frozen] IStringLocalizer<App> L1, [Frozen] IStringLocalizer<DateToString> L2, [Frozen] IStringLocalizer<CalendarBaseCard> L3)
    {
        Localize(L1, L2, L3);
        
        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCardTestPage>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton).Add(p => p.OnClickHistory, ClickButton));
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain(Icons.Material.Filled.History), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain(Icons.Material.Filled.Settings), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.MoreVert), TimeSpan.FromSeconds(2));
        await cut.Find("div.mud-menu button.mud-button-root").ClickAsync(new MouseEventArgs());
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.History), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.Settings), TimeSpan.FromSeconds(2));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain(Icons.Material.Filled.MoreVert), TimeSpan.FromSeconds(2));
    }

    private bool Clicked { get; set; }

    private void ClickButton()
    {
        Clicked = true;
    }

    private void Localize(IStringLocalizer<App> L1, IStringLocalizer<DateToString> L2, IStringLocalizer<CalendarBaseCard> L3)
    {
        Services.AddSingleton(L1);
        Services.AddSingleton(L2);
        Services.AddSingleton(L3);
        A.CallTo(() => L1["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));
        LocalizeA(L3, "Settings");
        LocalizeA(L3, "History");
        LocalizeA(L3, "Rate");
    }
}