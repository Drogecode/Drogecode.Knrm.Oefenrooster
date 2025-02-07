﻿using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class DateToStringTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void DateShrinkTodayTest([Frozen] IStringLocalizer<DateToString> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["Today"]).Returns(new LocalizedString("Today", "Today"));

        var today = DateTime.Today;
        var cut = RenderComponent<DateToString>(parameter => parameter.Add(p => p.DateTimeLocal, today).Add(p => p.ShowDayOfWeek, true).Add(p => p.ShowDate, true));
        cut.Markup.Should().Contain("Today");
        cut.Markup.Should().NotContain("Tomorrow");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void DateShrink8hour30minutesTest([Frozen] IStringLocalizer<DateToString> L)
    {
        Services.AddSingleton(L);
        var date = DateTime.Today.AddHours(8).AddMinutes(30);
        var cut = RenderComponent<DateToString>(parameter => parameter.Add(p => p.DateTimeLocal, date).Add(p => p.ShowDayOfWeek, true).Add(p => p.ShowTime, true));
        cut.Markup.Should().Contain("8:30");
        cut.Markup.Should().NotContain("8:31");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void DateShrinkLastYearTest([Frozen] IStringLocalizer<DateToString> L)
    {
        Services.AddSingleton(L);
        var date = new DateTime(2021, 12, 01, 7, 30, 15);
        var cut = RenderComponent<DateToString>(parameter => parameter.Add(p => p.DateTimeLocal, date).Add(p => p.ShowDayOfWeek, false).Add(p => p.ShowTime, true));
        cut.Markup.Should().Contain("7:30");
        cut.Markup.Should().NotContain("7:31");
        cut.Markup.Should().Contain("01 Dec 2021 7:30");
    }

    [Theory]
    [AutoFakeItEasyData]
    public void DateShrinkThisYearTest([Frozen] IStringLocalizer<DateToString> L)
    {
        Services.AddSingleton(L);
        var date = new DateTime(DateTime.Today.Year, 1, 1, 7, 30, 15);
        var cut = RenderComponent<DateToString>(parameter => parameter.Add(p => p.DateTimeLocal, date).Add(p => p.ShowDayOfWeek, true).Add(p => p.ShowTime, true));
        cut.Markup.Should().Contain("7:30");
        cut.Markup.Should().NotContain("7:31");
        cut.Markup.Should().Contain("01 Jan 7:30");
    }
}
