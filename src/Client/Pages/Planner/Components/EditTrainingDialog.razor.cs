using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class EditTrainingDialog : IDisposable
{
    [Inject] private IStringLocalizer<EditTrainingDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository ScheduleRepository { get; set; } = default!;
    [Inject] private VehicleRepository VehicleRepository { get; set; } = default!;
    [Inject] private IAuditClient AuditClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance? MudDialog { get; set; }
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
    private bool _isTaco;
#if DEBUG
    private const bool IS_DEBUG = true;
#else
    private const bool IS_DEBUG = false;
#endif
    private string[] _errors = Array.Empty<string>();
    [AllowNull] private MudForm _form = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Planner?.TrainingId is not null)
            {
                await SetExistingTraining();
            }
            else if (Planner?.DefaultId is not null)
            {
                await SetNewFromDefaultTraining();
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

            await SetRoleBasedVariables();

            StateHasChanged();
            MudDialog?.StateHasChanged();
        }
    }

    private async Task SetNewFromDefaultTraining()
    {
        var dateStartLocal = Planner!.DateStart.ToLocalTime();
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
        _linkVehicleTraining = await VehicleRepository.GetForDefaultAsync(Planner.DefaultId ?? throw new ArgumentNullException("Planner.DefaultId")) ?? [];
    }

    private async Task SetExistingTraining()
    {
        _linkVehicleTraining = await VehicleRepository.GetForTrainingAsync(Planner!.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
        var latestVersion = (await ScheduleRepository.GetPlannedTrainingById(Planner.TrainingId, _cls.Token))?.Training;
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
            Description = latestVersion?.Description,
            RoosterTrainingTypeId = Planner.RoosterTrainingTypeId,
            CountToTrainingTarget = Planner.CountToTrainingTarget,
            IsPinned = Planner.IsPinned,
            ShowTime = Planner.ShowTime,
        };
    }

    private async Task SetRoleBasedVariables()
    {
        _editOld = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_scheduler_edit_past);
        _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_super_user);

        if (_training?.IsNew ?? true)
        {
            _canEdit = true;
        }
        else
        {
            var dateEnd = DateTime.SpecifyKind(
                (_training.Date ?? throw new ArgumentNullException("_training.Date", "Date is null")) + (_training.TimeEnd ?? throw new ArgumentNullException($"TimeEnd is null")),
                DateTimeKind.Local).ToUniversalTime();
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
    }

    void Cancel() => MudDialog?.Cancel();

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

    private void RoosterTrainingTypeChanged(Guid? newType)
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
        await _form.Validate();
        if (!_form.IsValid || _training == null || !_canEdit)
        {
            DebugHelper.WriteLine($"Failed to save {!_form.IsValid} || {_training == null} || {!_canEdit}");
            await AuditClient.PostLogAsync(new PostLogRequest { Message = $"Failed to save {!_form.IsValid} || {_training == null} || {!_canEdit}" });
            return;
        }

        if (_training.TimeStart >= _training.TimeEnd) return;

        if (_training.IsNew || _training.IsNewFromDefault)
        {
            await UpdatePlannerObject();
            if (Planner is null)
            {
                DebugHelper.WriteLine("Planner should not be null");
                await AuditClient.PostLogAsync(new PostLogRequest { Message = "Planner should not be null" });
                return;
            }

            var newId = await ScheduleRepository.AddTraining(Planner, _cls.Token);
            if (_linkVehicleTraining is not null && _linkVehicleTraining.Count != 0)
            {
                foreach (var link in _linkVehicleTraining)
                {
                    link.RoosterTrainingId = newId;
                    await VehicleRepository.UpdateLinkVehicleTrainingAsync(link);
                }
            }

            _training.Id = newId;
            if (Refresh is not null)
                await Refresh.CallRequestRefreshAsync();
            if (_training.IsNew)
                await Global.CallNewTrainingAddedAsync(_training);
            _training.IsNew = false;
            _training.IsNewFromDefault = false;
            Planner.TrainingId = newId;
        }
        else if (Planner is not null)
        {
            await UpdatePlannerObject();
            await ScheduleRepository.PatchTraining(Planner, _cls.Token);
            if (Refresh is not null)
                await Refresh.CallRequestRefreshAsync();
        }
        else
        {
            DebugHelper.WriteLine("Failed to save Planner is null");
            await AuditClient.PostLogAsync(new PostLogRequest { Message = "Failed to save Planner is null" });
            return;
        }

        MudDialog?.Close(DialogResult.Ok(true));
    }

    private async Task UpdatePlannerObject()
    {
        if (_training is null)
        {
            DebugHelper.WriteLine("_training is null");
            await AuditClient.PostLogAsync(new PostLogRequest { Message = "_training is null" });
            return;
        }

        var dateStart = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeStart ?? throw new ArgumentNullException("TimeStart is null")),
            DateTimeKind.Local).ToUniversalTime();
        var dateEnd = DateTime.SpecifyKind((_training.Date ?? throw new ArgumentNullException("Date is null")) + (_training.TimeEnd ?? throw new ArgumentNullException("TimeEnd is null")),
            DateTimeKind.Local).ToUniversalTime();
        if (Planner is null)
        {
            DebugHelper.WriteLine("Planner is null, creating new planner object.");
            Planner = new PlannedTraining
            {
                DefaultId = _training.DefaultId,
                RoosterTrainingTypeId = _training.RoosterTrainingTypeId,
                Name = _training.Name,
                Description = _training.Description,
                DateStart = dateStart,
                DateEnd = dateEnd,
                CountToTrainingTarget = _training.CountToTrainingTarget,
                IsPinned = _training.IsPinned,
                ShowTime = _training.ShowTime,
                HasDescription = !string.IsNullOrWhiteSpace(_training.Description)
            };
        }
        else
        {
            DebugHelper.WriteLine("Updating planner object.");
            Planner.DefaultId = _training.DefaultId;
            Planner.RoosterTrainingTypeId = _training.RoosterTrainingTypeId;
            Planner.Name = _training.Name;
            Planner.Description = _training.Description;
            Planner.DateStart = dateStart;
            Planner.DateEnd = dateEnd;
            Planner.CountToTrainingTarget = _training.CountToTrainingTarget;
            Planner.IsPinned = _training.IsPinned;
            Planner.ShowTime = _training.ShowTime;
            Planner.HasDescription = !string.IsNullOrWhiteSpace(_training.Description);
        }
    }

    private async Task CheckChanged(bool toggled, DrogeVehicle vehicle)
    {
        if (!_canEdit) return;
        var link = _linkVehicleTraining?.FirstOrDefault(x => x.VehicleId == vehicle.Id);
        if (link is not null)
        {
            link.IsSelected = toggled;
            if (!_training!.IsNew && !_training!.IsNewFromDefault)
                await VehicleRepository.UpdateLinkVehicleTrainingAsync(link);
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
                var newLink = await VehicleRepository.UpdateLinkVehicleTrainingAsync(link);
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
            var users = Planner?.PlanUsers.Where(x => x.VehicleId == vehicle.Id);

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
        if (!_canEdit || Planner is null) return;
        if (_training?.IsNewFromDefault == true)
        {
            await UpdatePlannerObject();
            var newId = await ScheduleRepository.AddTraining(Planner, _cls.Token);
            Planner.TrainingId = newId;
            _training.Id = newId;
        }

        var result = await ScheduleRepository.DeleteTraining(_training?.Id, _cls.Token);
        if (result && Refresh is not null)
        {
            await Global.CallTrainingDeletedAsync(_training!.Id!.Value);
            await Refresh.CallRequestRefreshAsync();
            MudDialog?.Close(DialogResult.Ok(true));
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}