﻿using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleDialog : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; }
    [Parameter] public Oefenrooster.Shared.Models.Schedule.Planner Planner { get; set; } = default!;
    private CancellationTokenSource _cls = new();

    void Submit() => MudDialog.Close(DialogResult.Ok(true));
    void Cancel() => MudDialog.Cancel();

    private async Task CheckChangeddd(bool toggled, PlanUser user)
    {
        user.Assigned = toggled;
        await _scheduleRepository.PatchScheduleUserScheduled(Planner.TrainingId, user, _cls.Token);
    }

    private Color GetColor(Availabilty? availabilty)
    {
        switch (availabilty)
        {
            case Availabilty.Available:
                return Color.Success;
            case Availabilty.NotAvailable:
                return Color.Error;
            case Availabilty.Maybe:
                return Color.Warning;
            default: return Color.Inherit;
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}