using System.Diagnostics;
using Drogecode.Knrm.Oefenrooster.Shared.Providers.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Shared.Providers;

public class StopwatchProvider : IStopwatchProvider
{
    public static bool UseRealStopwatch
    {
        get
        {
            if (_useRealStopwatch is null) return false;
            if (_useRealStopwatch.Value > DateTime.UtcNow)
                return true;
            _useRealStopwatch = null;
            return false;
        }
        set
        {
            if (value)
            {
                _useRealStopwatch = DateTime.UtcNow.AddMinutes(15);
            }
            else
            {
                _useRealStopwatch = null;
            }
        }
    }

    private static DateTime? _useRealStopwatch;
    private readonly Stopwatch? _stopwatch;

    private StopwatchProvider()
    {
        if (UseRealStopwatch)
        {
            _stopwatch = new Stopwatch();
        }
    }

    public static StopwatchProvider StartNew()
    {
        var response = new StopwatchProvider();
        response.Start();
        return response;
    }

    private void Start()
    {
        _stopwatch?.Start();
    }

    public void Stop()
    {
        _stopwatch?.Stop();
    }

    public long ElapsedMilliseconds
    {
        get => _stopwatch?.ElapsedMilliseconds ?? -2;
    }
}