using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Schedule;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;

public sealed partial class EditTrainingDialog : IDisposable
{
    [Inject] private IStringLocalizer<EditTrainingDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [Inject] private VehicleRepository _vehicleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public PlannedTraining Planner { get; set; } = default!;
    [Parameter] public RefreshModel Refresh { get; set; } = default!;
    [Parameter] public List<DrogeVehicle>? Vehicles { get; set; }
    private CancellationTokenSource _cls = new();
    private List<DrogeLinkVehicleTraining>? _linkVehicleTraining;

    protected override async Task OnParametersSetAsync()
    {
        if (Planner.TrainingId != null)
            _linkVehicleTraining = await _vehicleRepository.GetForTrainingAsync(Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId"));
    }

    void Cancel() => MudDialog.Cancel();
    private async Task Submit()
    {

        MudDialog.Close(DialogResult.Ok(true));
    }

    private async Task CheckChanged(bool toggled, DrogeVehicle vehicle)
    {
        var d = _linkVehicleTraining?.FirstOrDefault(x => x.Vehicle == vehicle.Id);
        if (d != null)
        {
            Console.WriteLine("d is not null");
            d.IsSelected = toggled;
        }
        else
        {
            Console.WriteLine("d is null");
            _linkVehicleTraining?.Add(new DrogeLinkVehicleTraining
            {
                IsSelected = toggled,
                Vehicle = vehicle.Id,
                RoosterTrainingId = Planner.TrainingId ?? throw new ArgumentNullException("Planner.TrainingId")
            });
        }
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
