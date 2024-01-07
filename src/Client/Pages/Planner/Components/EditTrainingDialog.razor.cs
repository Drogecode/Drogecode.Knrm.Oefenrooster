﻿using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class EditTrainingDialog : IDisposable
{
    [Inject] private IStringLocalizer<EditTrainingDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public List<DrogeVehicle>? Vehicles { get; set; }
    [Parameter] public List<PlannerTrainingType>? TrainingTypes { get; set; }
    [Parameter] public PlannedTraining? Planner { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public DrogeCodeGlobal Global { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<DrogeLinkVehicleTraining>? _linkVehicleTraining;
    private EditTraining? _training;
    private PlannerTrainingType? _currentTrainingType;
    private bool _success;
    private bool _showDelete;
    private bool _startedWithShowNoTime;
    private bool _canEdit;
    private bool _showPadlock;
    private bool _editOld;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;
    protected override async Task OnParametersSetAsync()
    {
        if (Planner?.TrainingId is not null)
        {
            _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
            var dateStartLocal = Planner.DateStart.ToLocalTime();
            var dateEndLocal = Planner.DateEnd.ToLocalTime();
            _startedWithShowNoTime = !Planner.ShowTime;
            _training = new()
            {
                Id = Planner.TrainingId,
                Date = dateStartLocal.Date,
                TimeStart = dateStartLocal.TimeOfDay,
                TimeEnd = dateEndLocal.TimeOfDay,
                IsNew = false,
                Name = Planner.Name,
                RoosterTrainingTypeId = Planner.RoosterTrainingTypeId,
                CountToTrainingTarget = Planner.CountToTrainingTarget,
                IsPinned = Planner.IsPinned,
                ShowTime = Planner.ShowTime,
            };
        }
        else if (Planner?.DefaultId is not null)
        {
            var dateStartLocal = Planner.DateStart.ToLocalTime();
            var dateEndLocal = Planner.DateEnd.ToLocalTime();
            _startedWithShowNoTime = !Planner.ShowTime;
            _training = new()
            {
                IsNewFromDefault = true,
                IsNew = false,
                DefaultId = Planner.DefaultId,
                Date = dateStartLocal.Date,
                TimeStart = dateStartLocal.TimeOfDay,
                TimeEnd = dateEndLocal.TimeOfDay,
                Name = Planner.Name,
                RoosterTrainingTypeId = Planner.RoosterTrainingTypeId,
                CountToTrainingTarget = Planner.CountToTrainingTarget,
                IsPinned = Planner.IsPinned,
                ShowTime = Planner.ShowTime,
            };
            _linkVehicleTraining = new();
        }
        else
        {
            _training = new()
            {
                IsNew = true,
                ShowTime = true,
            };
            _linkVehicleTraining = new();
        }
        _currentTrainingType = TrainingTypes?.FirstOrDefault(x => x.Id == _training?.RoosterTrainingTypeId);

        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user != null)
            {
                _editOld = user.IsInRole(AccessesNames.AUTH_scheduler_edit_past);
            }
        }
        if (_training.IsNew)
        {
            _canEdit = true;
        }
        else
        {
            var dateEnd = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null")), DateTimeKind.Local).ToUniversalTime();
            if (dateEnd >= DateTime.UtcNow.AddDays(AccessesSettings.AUTH_scheduler_edit_past_days))
            {
                _canEdit = true;
            }
            else
            {
                _showPadlock = true;
                _canEdit = _editOld;
            }
        }
        if (!_canEdit)
        {
            _showPadlock = true;
        }
        MudDialog.StateHasChanged();
    }

    void Cancel() => MudDialog.Cancel();

    private string? DateValidation(DateTime? newDate)
    {
        if (_editOld)
            return null;
        if (newDate >= DateTime.UtcNow.AddDays(AccessesSettings.AUTH_scheduler_edit_past_days))
            return null;
        return L["To far in the past"];
    }
    private string? StartBeforeEndValidation(TimeSpan? timeStart)
    {
        if (_training?.TimeEnd < timeStart)
        {
            return L["Start time past end time"];
        }
        if (_training is not null && timeStart is not null && _training.TimeEnd is null)
        {
            _training!.TimeEnd = timeStart.Value.Add(new TimeSpan(2, 0, 0));
        }
        return null;
    }
    private string? EndAfterStartValidation(TimeSpan? timeEnd)
    {
        if (_training?.TimeStart >= timeEnd)
        {
            return L["End time before start time"];
        }
        return null;
    }

    private async Task RoosterTrainingTypeChanged(Guid? newType)
    {
        _training!.RoosterTrainingTypeId = newType;
        var trainingType = TrainingTypes!.FirstOrDefault(x => x.Id == newType);
        if (trainingType != null && trainingType.CountToTrainingTarget != _training!.CountToTrainingTarget)
        {
            _training.CountToTrainingTarget = trainingType.CountToTrainingTarget;
        }
    }

    private async Task OnSubmit()
    {
        _form?.Validate();
        if (!_form?.IsValid == true || _training == null || !_canEdit) return;
        if (_training.TimeStart >= _training.TimeEnd) return;

        if (_training.IsNew || _training.IsNewFromDefault)
        {
            UpdatePlannerObject();
            var newId = await _scheduleRepository.AddTraining(Planner, _cls.Token);
            if (_linkVehicleTraining is not null)
            {
                foreach (var link in _linkVehicleTraining)
                {
                    link.RoosterTrainingId = newId;
                    await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
                }
            }
            if (Refresh is not null)
                await Refresh.CallRequestRefreshAsync();
            if (_training.IsNew)
                await Global.CallNewTrainingAddedAsync(_training);
            _training.Id = newId;
            _training.IsNew = false;
            _training.IsNewFromDefault = false;
            Planner.TrainingId = newId;
        }
        else if (Planner is not null)
        {
            UpdatePlannerObject();
            await _scheduleRepository.PatchTraining(Planner, _cls.Token);
            if (Refresh is not null)
                await Refresh.CallRequestRefreshAsync();
        }
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void UpdatePlannerObject()
    {
        var dateStart = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeStart ?? throw new ArgumentNullException("TimeStart is null")), DateTimeKind.Local).ToUniversalTime();
        var dateEnd = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null")), DateTimeKind.Local).ToUniversalTime();
        if (Planner is null)
        {
            Planner = new PlannedTraining
            {
                DefaultId = _training.DefaultId,
                RoosterTrainingTypeId = _training.RoosterTrainingTypeId,
                Name = _training.Name,
                DateStart = dateStart,
                DateEnd = dateEnd,
                CountToTrainingTarget = _training.CountToTrainingTarget,
                IsPinned = _training.IsPinned,
                ShowTime = _training.ShowTime,
            };
        }
        else
        {
            Planner.DefaultId = _training.DefaultId;
            Planner.RoosterTrainingTypeId = _training.RoosterTrainingTypeId;
            Planner.Name = _training.Name;
            Planner.DateStart = dateStart;
            Planner.DateEnd = dateEnd;
            Planner.CountToTrainingTarget = _training.CountToTrainingTarget;
            Planner.IsPinned = _training.IsPinned;
            Planner.ShowTime = _training.ShowTime;
        }
    }

    private async Task CheckChanged(bool toggled, DrogeVehicle vehicle)
    {
        if (!_canEdit) return;
        var link = _linkVehicleTraining?.FirstOrDefault(x => x.VehicleId == vehicle.Id);
        if (link is not null)
        {
            link.IsSelected = toggled;
            if (!_training!.IsNew)
                await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
        }
        else
        {
            link = new DrogeLinkVehicleTraining
            {
                IsSelected = toggled,
                VehicleId = vehicle.Id,
                RoosterTrainingId = Planner?.TrainingId ?? Guid.Empty
            };
            if (!_training!.IsNew && !_training!.IsNewFromDefault)
            {
                var newLink = await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
                if (newLink is not null)
                    _linkVehicleTraining?.Add(newLink);
            }
            else
            {
                _linkVehicleTraining?.Add(link);
            }
        }
        if (!toggled)
        {
            var users = Planner?.PlanUsers?.Where(x => x.VehicleId == vehicle.Id);

            if (users is not null)
            {
                foreach (var user in users)
                {
                    user.VehicleId = null;
                }
            }
            if (Refresh is not null)
                await Refresh.CallRequestRefreshAsync();
            StateHasChanged();
        }
    }

    private async Task Delete()
    {
        if (!_canEdit) return;
        if (_training?.IsNewFromDefault == true)
        {
            UpdatePlannerObject();
            var newId = await _scheduleRepository.AddTraining(Planner, _cls.Token);
            Planner.TrainingId = newId;
            _training.Id = newId;
        }
        var result = await _scheduleRepository.DeleteTraining(_training?.Id, _cls.Token);
        if (result && Refresh is not null)
        {
            await Global.CallTrainingDeletedAsync(_training!.Id!.Value);
            await Refresh.CallRequestRefreshAsync();
            MudDialog.Close(DialogResult.Ok(true));
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
