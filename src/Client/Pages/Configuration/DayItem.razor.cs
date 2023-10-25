using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DayItem : IDisposable
{
    [Inject] private ICalendarItemClient _calendarItemClient { get; set; } = default!;
    private CancellationTokenSource _cls = new();

    protected override void OnInitialized()
    {
        var items = _calendarItemClient.GetAllFutureDayItemsAsync(30, 0);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
