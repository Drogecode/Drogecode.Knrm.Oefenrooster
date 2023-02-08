﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleBlock : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleBlock> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private ISnackbar SnackbarService { get; set; } = default!;

    [Parameter, EditorRequired] public PlannedTraining Planner { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; }
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; }
    private RefreshModel _refreshModel = new();
    private bool _updating;

    protected override void OnParametersSet()
    {
        _refreshModel.RefreshRequested += RefreshMe;
    }

    private void OpenDialog()
    {
        if (Planner.IsCreated)
        {
            DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
            _dialogProvider.Show<ScheduleDialog>(L["Schedule people for this training"], new DialogParameters {
            { "Planner", Planner },
            { "Refresh", _refreshModel },
            { "Users", Users },
            { "Functions", Functions}
            }, options);
        }
        else
        {
            SnackbarService.Add(L["Requires one person to set their availability"]);
        }
    }

    private void RefreshMe()
    {
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequested -= RefreshMe;
    }
}
