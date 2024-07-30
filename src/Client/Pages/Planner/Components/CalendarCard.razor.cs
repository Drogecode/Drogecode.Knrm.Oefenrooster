using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class CalendarCard : IDisposable
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository ScheduleRepository { get; set; } = default!;
    [Parameter, EditorRequired] public Training Training { get; set; } = default!;
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public string? MinWidth { get; set; }
    [Parameter] public string? MaxWidth { get; set; }
    [Parameter] public bool ShowDate { get; set; }
    [Parameter] public bool ShowDayOfWeek { get; set; }
    private CancellationTokenSource _cls = new();
    private bool _updating;

    protected override void OnParametersSet()
    {
        if (Training.Availability == Availability.None)
            Training.Availability = null;
    }

    private async Task OnChange()
    {
        if (_updating) return;
        _updating = true;
        Training.SetBy = AvailabilitySetBy.User;
        var updatedTraining = await ScheduleRepository.PatchScheduleForUser(Training, _cls.Token);
        if (updatedTraining is not null)
            Training = updatedTraining;
        _updating = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}