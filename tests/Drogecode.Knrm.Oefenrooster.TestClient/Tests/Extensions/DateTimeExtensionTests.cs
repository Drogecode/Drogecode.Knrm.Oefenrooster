using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Extensions;
using Drogecode.Knrm.Oefenrooster.TestClient.Attributes;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Extensions;

public class DateTimeExtensionTests
{
    [Theory]
    [AutoFakeItEasyData]
    public void TodayToNiceString([Frozen] IStringLocalizer<DateToString> L1)
    {
        Localize(L1);
        var niceString = DateTime.Today.ToNiceString(L1);
        niceString.Should().Be("Today 0:00");
    }
    [Theory]
    [AutoFakeItEasyData]
    public void TodayToNiceStringWithNoTime([Frozen] IStringLocalizer<DateToString> L1)
    {
        Localize(L1);
        var niceString = DateTime.Today.ToNiceString(L1, showtime: false);
        niceString.Should().Be("Today");
    }

    private void Localize(IStringLocalizer<DateToString> L1)
    {
        A.CallTo(() => L1["Yesterday"]).Returns(new LocalizedString("Yesterday", "Yesterday"));
        A.CallTo(() => L1["Today"]).Returns(new LocalizedString("Today", "Today"));
        A.CallTo(() => L1["Tomorrow"]).Returns(new LocalizedString("Tomorrow", "Tomorrow"));
    }
}
