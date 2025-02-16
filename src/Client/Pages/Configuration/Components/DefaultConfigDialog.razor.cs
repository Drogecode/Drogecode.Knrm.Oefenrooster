using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class DefaultConfigDialog : IDisposable
{
    [Inject] private IStringLocalizer<DefaultConfig> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository DefaultScheduleRepository { get; set; } = default!;
    [Inject] private ITrainingTypesClient TrainingTypesClient { get; set; } = default!;
    [Inject, NotNull] private VehicleRepository? VehicleRepository { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DefaultSchedule? DefaultSchedule { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private List<PlannerTrainingType>? _trainingTypes { get; set; }
    private List<DrogeVehicle>? _vehicles { get; set; }
    private IEnumerable<Guid> _selectedVehicleIds { get; set; }
    private PlannerTrainingType? _currentTrainingType;
    private readonly CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _trainingTypes = (await TrainingTypesClient.GetTrainingTypesAsync(false, _cls.Token)).PlannerTrainingTypes;
            _vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
            if (IsNew == true || DefaultSchedule is null)
            {
                DefaultSchedule = new DefaultSchedule();
            }

            if (DefaultSchedule.RoosterTrainingTypeId is not null)
            {
                _currentTrainingType = (await TrainingTypesClient.GetByIdAsync(DefaultSchedule.RoosterTrainingTypeId.Value, _cls.Token)).TrainingType;
            }

            _selectedVehicleIds = DefaultSchedule.VehicleIds ?? [];

            StateHasChanged();
        }
    }

    public string? ValidateStartDate(DateTime? newValue)
    {
        if (newValue == DefaultSchedule?.ValidFromDefault) return null;
        if (newValue == null) return L["No value for start date"];
        if (newValue.Value.CompareTo(DateTime.UtcNow.Date) < 0) return L["Should not be in the past"];
        return null;
    }

    public string? ValidateTillDate(DateTime? newValue)
    {
        if (newValue == DefaultSchedule?.ValidUntilDefault) return null;
        if (newValue == null || DefaultSchedule is null) return L["No value for till date"];
        if (newValue.Value.CompareTo(DefaultSchedule.ValidFromDefault) < 0) return L["Should not be before start date"];
        return null;
    }

    private async Task Submit()
    {
        if (DefaultSchedule is null) return;
        DefaultSchedule.VehicleIds = _selectedVehicleIds.ToList();
        if (IsNew == true)
        {
            var newResult = await DefaultScheduleRepository.PutDefaultScheduleAsync(DefaultSchedule, _cls.Token);
            if (newResult.NewId is not null)
            {
                DefaultSchedule.Id = newResult.NewId.Value;
            }
        }
        else
        {
            var patchResult = await DefaultScheduleRepository.PatchDefaultScheduleAsync(DefaultSchedule, _cls.Token);
            if (patchResult?.Success != true)
            {
                return;
            }
        }

        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }

    private string? VehicleIdToString(Guid vehicleId)
    {
        var vehicle = _vehicles?.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle is null)
            return L["Vehicle found"];
        return vehicle.Name;
    }
}