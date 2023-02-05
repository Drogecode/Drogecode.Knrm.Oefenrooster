using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class Schedule : IDisposable
{
    [Inject] private IStringLocalizer<Schedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter] public Guid CustomerId { get; set; } = Guid.Empty;
    private LinkedList<List<Oefenrooster.Shared.Models.Schedule.Planner>> _scheduleForUser = new();
    private CancellationTokenSource _cls = new();
    private int? _month;
    private int _high = -1;
    private int _low = -2;

    protected override async Task OnInitializedAsync()
    {
        for (int i = -1; i < 6; i++)
        {
            await AddWeekToSchadules(true);
        }
    }

    private async Task AddWeekToSchadules(bool high)
    {
        List<Oefenrooster.Shared.Models.Schedule.Planner>? scheduleForUser = null;
        var PlannersInWeek = (await _scheduleRepository.ScheduleForAll(high ? _high : _low, _cls.Token))?.Planners;
        if (PlannersInWeek != null)
        {
            scheduleForUser = new List<Oefenrooster.Shared.Models.Schedule.Planner>();
            foreach (var Plan in PlannersInWeek)
                scheduleForUser.Add(Plan);
        }
        if (high)
        {
            if (scheduleForUser != null)
                _scheduleForUser.AddLast(scheduleForUser);
            _high++;
        }
        else
        {
            if (scheduleForUser != null)
                _scheduleForUser.AddFirst(scheduleForUser);
            _low--;
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Dispose();
    }
}
