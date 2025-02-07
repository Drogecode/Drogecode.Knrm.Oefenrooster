using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Helpers;

public class PlannerHelperTests
{
    [Theory]
    [InlineData(Availability.Available, Color.Success)]
    [InlineData(Availability.NotAvailable, Color.Error)]
    [InlineData(Availability.Maybe, Color.Warning)]
    [InlineData(Availability.None, Color.Inherit)]
    [InlineData(null, Color.Inherit)]
    public void ColorAvailabiltyTests(Availability? value, Color expected)
    {
        var result = PlannerHelper.ColorAvailabilty(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Availability.Available, "bg-green-400")]
    [InlineData(Availability.NotAvailable, "bg-red-400")]
    [InlineData(Availability.Maybe, "bg-orange-400")]
    [InlineData(Availability.None, "")]
    [InlineData(null, "")]
    public void StyleAvailabiltyTests(Availability? value, string expected)
    {
        var result = PlannerHelper.StyleAvailabilty(value);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ForMonthTest()
    {
        var range = new DateRange
        {
            Start = new DateTime(2023,5,29),
            End = new DateTime(2023,7,2)
        };
        var result = PlannerHelper.ForMonth(range);
        result.Should().Be(new DateTime(2023,6,1));
    }
}
