using Microsoft.Extensions.Logging;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.PreCom;

public class Sleeper
{
    private readonly ILogger _logger;

    public Sleeper(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<NextRunMode> Sleep(DateTime lastRun)
    {
        if (lastRun.Date.CompareTo(DateTime.Today) == 0)
        {
            var nextRun = lastRun.AddHours(1);
            nextRun = nextRun.AddMinutes(-nextRun.Minute);
            while (true)
            {
                if (nextRun.CompareTo(DateTime.Now) <= 0)
                {
                    return NextMode(nextRun);
                }
                await Task.Delay(1000);
            }
        }
        else
        {
            await SleepShort();
        }
        return NextMode(DateTime.Now);
    }

    public NextRunMode NextMode(DateTime nextRun)
    {
        switch (nextRun.DayOfWeek)
        {
            case DayOfWeek.Friday:
                if (nextRun.Hour == 19) return NextRunMode.TodayTomorrow;
                else if (nextRun.Hour == 20) return NextRunMode.NextWeek;
                break;
            case DayOfWeek.Saturday:
                if (nextRun.Hour == 19) return NextRunMode.NextWeek;
                break;
            default:
                if (nextRun.Hour == 19) return NextRunMode.TodayTomorrow;
                break;
        }
        if (nextRun.Hour > 7 && nextRun.Hour < 22) return NextRunMode.NextHour;
        else return NextRunMode.None; // No messages while night time!
    }

    public async Task SleepShort()
    {
        for (int i = 0; i < 60; i++)
        {
            await Task.Delay(1001);
        }
    }
}

