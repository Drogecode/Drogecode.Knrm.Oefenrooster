using Drogecode.Knrm.Oefenrooster.Client;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Components.DrogeCode;

public class MonthSelectorTests : BlazorTestBase
{
    [Theory]
    [AutoFakeItEasyData]
    public void MonthThisYearTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["Today"]).Returns(new LocalizedString("Today", "Today"));
        
        var month = DateTime.Today;
        var cut = RenderComponent<MonthSelector>(parameter => parameter.Add(p => p.Month, month));
        cut.Markup.Should().Contain(month.ToString("MMMM"));
        cut.Markup.Should().NotContain(month.Year.ToString());
    }

    [Theory]
    [AutoFakeItEasyData]
    public void MonthNotThisYearTest([Frozen] IStringLocalizer<App> L)
    {
        Services.AddSingleton(L);
        A.CallTo(() => L["Today"]).Returns(new LocalizedString("Today", "Today"));
        
        var month = new DateTime(2022, 11, 5);
        var cut = RenderComponent<MonthSelector>(parameter => parameter.Add(p => p.Month, month));
        cut.Markup.Should().Contain(month.ToString("MMMM"));
        cut.Markup.Should().Contain(month.Year.ToString());
    }
}
