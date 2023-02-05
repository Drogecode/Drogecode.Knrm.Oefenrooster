﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleBlock : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleBlock> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Parameter, EditorRequired] public Oefenrooster.Shared.Models.Schedule.Planner Planner { get; set; } = default!;
    private RefreshModel _refreshModel = new();
    private bool _updating;

    protected override void OnParametersSet()
    {
        _refreshModel.RefreshRequested += RefreshMe;
    }

    private void OpenDialog()
    {
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<ScheduleDialog>(L["Schedule people for this training"], new DialogParameters { { "Planner", Planner }, { "Refresh", _refreshModel } }, options);
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
