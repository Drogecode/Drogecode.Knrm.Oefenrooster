using AutoFixture.Xunit2;
using Bunit;
using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule.Abstract;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;
using FakeItEasy;
using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class CalendarBaseCardTests : TestContext
{
    [Theory]
    [AutoFakeItEasyData]
    public void HasNameTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["till"]).Returns(new LocalizedString("till", "till with some more text to ensure it is replaced"));

        var training = new TrainingAdvance { Name = "xUnit meets bUnit" };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().Contain("xUnit meets bUnit");
        cut.Markup.Should().Contain("till with some more text to ensure it is replaced");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameButDayTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["till"]).Returns(new LocalizedString("till", "till"));

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.ReplaceEmtyName, true));
        cut.Markup.Should().Contain(training.DateStart.ToLocalTime().ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void NoNameNoReplaceTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["till"]).Returns(new LocalizedString("till", "till"));

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training));
        cut.Markup.Should().NotContain(training.DateStart.ToLocalTime().ToString("dddd"));
    }

    [Theory]
    [AutoFakeItEasyData]
    public void OnClickHistoryTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["till"]).Returns(new LocalizedString("till", "till"));

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickHistory, ClickButton));
        var d = cut.Find(".DrogeCode-card-header-history");
        d.Click();
        Assert.True(Clicked);
    }
    [Theory]
    [AutoFakeItEasyData]
    public void OnClickSettingsTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["till"]).Returns(new LocalizedString("till", "till"));

        var training = new TrainingAdvance { DateStart = DateTime.UtcNow };
        var cut = RenderComponent<CalendarBaseCard>(parameter => parameter.Add(p => p.Training, training).Add(p => p.OnClickSettings, ClickButton));
        var d = cut.Find(".DrogeCode-card-header-settings");
        d.Click();
        Assert.True(Clicked);
    }
    private bool Clicked = false;
    private void ClickButton()
    {
        Clicked = true;
    }
}
