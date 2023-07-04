using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.PreCom.Enums;
using Microsoft.Extensions.Logging;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom.Tests;

public class SleeperTests
{
    private readonly ILogger _logger;
    public SleeperTests(ILogger logger)
    {
        _logger = logger;
    }

    [Fact]
    public void NextModeMidNightTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 5)); // monday
        result.Should().Be(NextRunMode.None);
    }

    [Fact]
    public void NextModeFridayTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 9, 20, 0, 0)); // friday
        result.Should().Be(NextRunMode.NextWeek);
    }
}