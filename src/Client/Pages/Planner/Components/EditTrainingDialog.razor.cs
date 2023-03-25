using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;
using System.Diagnostics.CodeAnalysis;

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
    private bool _success;
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form;
    protected override async Task OnParametersSetAsync()
    {
        if (Planner?.TrainingId != null)
        {
            _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
            _training = new()
            {
                Id = Planner.TrainingId,
                Date = Planner.DateStart.Date,
                TimeStart = Planner.DateStart.TimeOfDay,
                TimeEnd = Planner.DateEnd.TimeOfDay,
                IsNew = false,
                Name = Planner.Name,
                RoosterTrainingTypeId = Planner.RoosterTrainingTypeId,
                CountToTrainingTarget = Planner.CountToTrainingTarget,
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
    }

    void Cancel() => MudDialog.Cancel();

    private string? StartBeforeEndValidation(TimeSpan? timeStart)
    {
        if (_training?.TimeEnd < timeStart)
        {
            return L["Start time past end time"];
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

    private async Task OnSubmit()
    {
        _form?.Validate();
        if (!_form?.IsValid == true || _training == null) return;
        if (_training.TimeStart >= _training.TimeEnd) return;

        if (_training.IsNew)
        {
            var newId = await _scheduleRepository.AddTraining(_training, _cls.Token);
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
            await _scheduleRepository.PatchTraining(_training, _cls.Token);
            var dateStart = (_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeStart ?? throw new ArgumentNullException("TimeStart is null"));
            var dateEnd = (_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null"));
            Planner.RoosterTrainingTypeId = _training.RoosterTrainingTypeId;
            Planner.Name = _training.Name;
            Planner.DateStart = dateStart;
            Planner.DateEnd = dateEnd;
            if (Refresh != null)
                await Refresh.CallRequestRefreshAsync();
        }
        MudDialog.Close(DialogResult.Ok(true));
    }

    private async Task CheckChanged(bool toggled, DrogeVehicle vehicle)
    {
        var link = _linkVehicleTraining?.FirstOrDefault(x => x.Vehicle == vehicle.Id);
        if (link != null)
        {
            Console.WriteLine("d is not null");
            link.IsSelected = toggled;
            if (!_training!.IsNew)
                await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
        }
        else
        {
            Console.WriteLine("d is null");
            link = new DrogeLinkVehicleTraining
            {
                Id = null,
                IsSelected = toggled,
                Vehicle = vehicle.Id,
                RoosterTrainingId = Planner?.TrainingId ?? Guid.Empty
            };
            if (!_training!.IsNew)
            {
                var newLink = await _vehicleRepository.UpdateLinkVehicleTrainingAsync(link);
                if (newLink != null)
                    _linkVehicleTraining?.Add(newLink);
                else
                    Console.WriteLine("Failed to add new link");
            }
            else
            {
                _linkVehicleTraining?.Add(link);
            }
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
