using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.TrainingTypes;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;

public sealed partial class TrainingDetails : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<TrainingDetails>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private ScheduleRepository? ScheduleRepository { get; set; }
    [Inject, NotNull] private VehicleRepository? VehicleRepository { get; set; }
    [Inject, NotNull] private TrainingTypesRepository? TrainingTypesRepository { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [CascadingParameter] private DrogeCodeGlobal Global { get; set; } = default!;
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private PlannedTraining? _training = null;
    private List<PlannerTrainingType>? _trainingTypes;
    private List<DrogeVehicle>? _vehicles;
    private bool _showHistory;
    private bool _loading = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _training = (await ScheduleRepository.GetPlannedTrainingById(Id, _cls.Token))?.Training;
            _trainingTypes = await TrainingTypesRepository.GetTrainingTypes(false, false, _cls.Token);
            _vehicles = await VehicleRepository.GetAllVehiclesAsync(false, _cls.Token);
            _showHistory = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_scheduler_history);
            _loading = false;
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}