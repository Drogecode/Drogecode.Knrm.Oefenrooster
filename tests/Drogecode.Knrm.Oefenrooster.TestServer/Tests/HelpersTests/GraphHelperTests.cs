using Drogecode.Knrm.Oefenrooster.Server.Helpers;

namespace Drogecode.Knrm.Oefenrooster.TestServer.Tests.HelpersTests;

public class GraphHelperTests
{
    [Theory]
    [InlineData("1.0 dddd", 1.0)]
    [InlineData("107 - Prio 1, Surfer in problemen", 107)]
    [InlineData("78a - waterongeval oostkade", 78.1)]
    [InlineData("78.5a - ONE MORE", 78.5)]
    [InlineData("76B - KAC, Vaartuig aan de grond", 76.2)]
    [InlineData("69actie", 69)]
    [InlineData("125A- KNRM app melding. Boot met kapotte brandstofpomp nabij Hollandsebrug", 125.1)]
    public async Task FirstNumberFromStringTest(string description, double result)
    {
        var d = GraphHelper.FirstNumberFromString(description);
        d.Should().Be(result);
    }
}