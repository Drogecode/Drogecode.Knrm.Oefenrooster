using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class CalendarBlock : IDisposable
{
    [Inject] private IStringLocalizer<CalendarBlock> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Parameter, EditorRequired] public Training Training { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private bool _updating;
    private Color ColorHeader
    {
        get
        {
            switch (Training.Availabilty)
            {
                case Availabilty.Available:
                    return Color.Success;
                case Availabilty.NotAvailable:
                    return Color.Error;
                case Availabilty.Maybe:
                    return Color.Warning;
                case Availabilty.None:
                default:
                    return Color.Inherit;
            }
        }
    }

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
