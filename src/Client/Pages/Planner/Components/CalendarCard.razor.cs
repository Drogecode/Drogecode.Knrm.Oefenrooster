using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class CalendarCard : IDisposable
{
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter, EditorRequired] public Training Training { get; set; } = default!;
    [Parameter] public string Width { get; set; } = "100%";
    [Parameter] public bool ShowDate { get; set; } = false;
    private CancellationTokenSource _cls = new();
    private bool _updating;

    protected override void OnParametersSet()
    {
        if (Training.Availabilty == Availabilty.None)
            Training.Availabilty = null;
    }

    private async Task OnChange()
    {
        if (_updating) return;
        _updating = true;
        var updatedTraining = await _scheduleRepository.PatchScheduleForUser(Training, _cls.Token);
        Training = updatedTraining;
        _updating = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
