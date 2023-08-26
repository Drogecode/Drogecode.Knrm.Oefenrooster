using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class EditTrainingDialog : IDisposable
{
    [Inject] private IStringLocalizer<EditTrainingDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
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
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;
    protected override async Task OnParametersSetAsync()
    {
        if (Planner?.TrainingId != null)
        {
            _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
            var dateStartLocal = Planner.DateStart.ToLocalTime();
            var dateEndLocal = Planner.DateEnd.ToLocalTime();
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
                Pin = Planner.IsPinned
            };
        }
        else
        {
            _training = new()
            {
                IsNew = true
            };
            _linkVehicleTraining = new();
        }
        _currentTrainingType = TrainingTypes?.FirstOrDefault(x => x.Id == _training?.RoosterTrainingTypeId);
    }

    void Cancel() => MudDialog.Cancel();

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
        if (!_form?.IsValid == true || _training == null) return;
        if (_training.TimeStart >= _training.TimeEnd) return;

        var dateStart = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeStart ?? throw new ArgumentNullException("TimeStart is null")), DateTimeKind.Local).ToUniversalTime();
        var dateEnd = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null")), DateTimeKind.Local).ToUniversalTime();
        if (_training.IsNew)
        {
            Planner = new PlannedTraining
            {
                RoosterTrainingTypeId = _training.RoosterTrainingTypeId,
                Name = _training.Name,
                DateStart = dateStart,
                DateEnd = dateEnd,
                CountToTrainingTarget = _training.CountToTrainingTarget,
                IsPinned = _training.Pin,
            };
            var newId = await _scheduleRepository.AddTraining(Planner, _cls.Token);
            _training.Id = newId;
            if (_linkVehicleTraining != null)
            {
                foreach (var link in _linkVehicleTraining)
                {
                    link.RoosterTrainingId = newId;
                    await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
                }
            }
            await Global.CallNewTrainingAddedAsync(_training);
        }
        else if (Planner != null)
        {
            Planner.RoosterTrainingTypeId = _training.RoosterTrainingTypeId;
            Planner.Name = _training.Name;
            Planner.DateStart = dateStart;
            Planner.DateEnd = dateEnd;
            Planner.CountToTrainingTarget = _training.CountToTrainingTarget;
            Planner.IsPinned = _training.Pin;
            await _scheduleRepository.PatchTraining(Planner, _cls.Token);
            if (Refresh != null)
                await Refresh.CallRequestRefreshAsync();
        }
        MudDialog.Close(DialogResult.Ok(true));
    }

    private async Task CheckChanged(bool toggled, DrogeVehicle vehicle)
    {
        var link = _linkVehicleTraining?.FirstOrDefault(x => x.VehicleId == vehicle.Id);
        if (link != null)
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
            if (!_training!.IsNew)
            {
                var newLink = await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
                if (newLink != null)
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
            if (Refresh != null)
                await Refresh.CallRequestRefreshAsync();
            StateHasChanged();
        }
    }

    private async Task Delete()
    {
        var result = await _scheduleRepository.DeleteTraining(_training?.Id, _cls.Token);
        if (result && Refresh != null)
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
