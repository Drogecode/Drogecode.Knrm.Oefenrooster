using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
public sealed partial class Calendar : IDisposable
{
    [Inject] private IStringLocalizer<Calendar> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private LinkedList<List<Training>> _calendarForUser = new();
    private CancellationTokenSource _cls = new();
    private int? _month;
    private int _high = -1;
    private int _low = -2;

    protected override async Task OnInitializedAsync()
    {
        for (int i = -1; i < 6; i++)
        {
            await AddWeekToCalendar(true);
        }
    }

    private async Task AddWeekToCalendar(bool high)
    {
        List<Training> scheduleForUser = new();
        var trainingsInWeek = (await _scheduleRepository.CalendarForUser(high ? _high : _low, _cls.Token))?.Trainings;
        if (trainingsInWeek != null)
            foreach (var training in trainingsInWeek)
                scheduleForUser.Add(training);
        if (high)
        {
            _calendarForUser.AddLast(scheduleForUser);
            _high++;
        }
        else
        {
            _calendarForUser.AddFirst(scheduleForUser);
            _low--;
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
