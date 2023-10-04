﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using static MudBlazor.CategoryTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class ScheduleTable : IDisposable
{
    [Inject] private IStringLocalizer<ScheduleTable> L { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter, EditorRequired] public List<DrogeUser>? Users { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction>? Functions { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeVehicle>? Vehicles { get; set; } = default!;
    [Parameter, EditorRequired] public List<PlannerTrainingType>? TrainingTypes { get; set; } = default!;
    [Parameter, EditorRequired] public Schedule Parent { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private bool _updating;
    private bool _canEdit;
    private bool _working;
    private List<PlannedTraining> _events = new();
    private List<UserTrainingCounter>? _userTrainingCounter;
    private DateTime? _month;

    private async Task SetMonth(DateTime? dateTime)
    {
        if (dateTime == null) return;
        _month = dateTime;
        DateRange dateRange = new DateRange
        {
            Start = new DateTime(dateTime.Value.Year, dateTime.Value.Month, 1),
            End = new DateTime(dateTime.Value.Year, dateTime.Value.Month, DateTime.DaysInMonth(dateTime.Value.Year, dateTime.Value.Month))
        };
        await SetCalenderForMonth(dateRange);
    }

    protected override async Task OnInitializedAsync()
    {
        await SetMonth(DateTime.Today);
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user is not null)
            {
                _canEdit = user.IsInRole(AccessesNames.AUTH_scheduler_in_table_view);
            }
        }
    }

    private async Task SetCalenderForMonth(DateRange dateRange)
    {
        if (_updating || dateRange.Start == null) return;
        _updating = true;
        _events = new();
        TrainingWeek scheduleForUser = new();
        var scheduleForAll = await _scheduleRepository.ScheduleForAll(dateRange, true, _cls.Token);
        if (scheduleForAll == null) return;
        _userTrainingCounter = scheduleForAll.UserTrainingCounters;
        var trainingsInRange = scheduleForAll.Planners;
        if (trainingsInRange != null && trainingsInRange.Count > 0)
        {
            scheduleForUser.From = DateOnly.FromDateTime(trainingsInRange[0].DateStart);
            foreach (var training in trainingsInRange)
            {
                _events.Add(training);
            }
        }
        _updating = false;
        StateHasChanged();
    }

    private async Task Click(PlanUser? user, PlannedTraining? training)
    {
        if (!_canEdit || _working || user is null || training is null) return;
        _working = true;
        user.Assigned = !user.Assigned;
        await _scheduleRepository.PatchAssignedUser(training.TrainingId, training, user);
        Parent.ShowSnackbarAssignmentChanged(user, training);
        StateHasChanged();
        _working = false;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
