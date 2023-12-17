using Drogecode.Knrm.Oefenrooster.PreCom;
using Drogecode.Knrm.Oefenrooster.PreCom.Enums;
using Microsoft.Extensions.Logging;

namespace Drogecode.Knrm.Oefenrooster.TestPreCom.Tests;

public class SleeperTests
{
    private readonly ILogger _logger;
    public SleeperTests(ILoggerProvider loggerProvider)
    {
        _logger = loggerProvider.CreateLogger("SleeperTests");
    }

    [Fact]
    public void NextModeMidNightTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 5)); // monday
        result.Should().Be(NextRunMode.None);
    }

    [Fact]
    public void NextModeFriday8AMTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 9, 8, 0, 0)); // friday
        result.Should().Be(NextRunMode.NextHour);
    }

    [Fact]
    public void NextModeFriday7PMTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 9, 19, 0, 0)); // friday
        result.Should().Be(NextRunMode.TodayTomorrow);
    }

    [Fact]
    public void NextModeFriday8PMTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 9, 20, 0, 0)); // friday
        result.Should().Be(NextRunMode.NextWeek);
    }

    [Fact]
    public void NextModeTuseday7PMTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 6, 19, 0, 0)); // tuseday
        result.Should().Be(NextRunMode.TodayTomorrow);
    }

    [Fact]
    public void NextModeTuseday8PMTest()
    {
        var sleeper = new Sleeper(_logger);
        var result = sleeper.NextMode(new DateTime(2018, 11, 6, 20, 0, 0)); // tuseday
        result.Should().Be(NextRunMode.NextHour);
    }
}