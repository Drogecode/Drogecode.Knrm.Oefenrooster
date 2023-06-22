using Drogecode.Knrm.Oefenrooster.Client.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using FluentAssertions;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Tests.Helpers;

public class PlannerHelperTests
{
    [Theory]
    [InlineData(Availabilty.Available, Color.Success)]
    [InlineData(Availabilty.NotAvailable, Color.Error)]
    [InlineData(Availabilty.Maybe, Color.Warning)]
    [InlineData(Availabilty.None, Color.Inherit)]
    [InlineData(null, Color.Inherit)]
    public void ColorAvailabiltyTests(Availabilty? value, Color expected)
    {
        var result = PlannerHelper.ColorAvailabilty(value);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(Availabilty.Available, "bg-green-400")]
    [InlineData(Availabilty.NotAvailable, "bg-red-400")]
    [InlineData(Availabilty.Maybe, "bg-orange-400")]
    [InlineData(Availabilty.None, "")]
    [InlineData(null, "")]
    public void StyleAvailabiltyTests(Availabilty? value, string expected)
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
