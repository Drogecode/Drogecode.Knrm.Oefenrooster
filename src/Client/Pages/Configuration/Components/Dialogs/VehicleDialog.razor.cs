using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

public sealed partial class VehicleDialog : IDisposable
{
    [Inject] private IStringLocalizer<Vehicles> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IVehicleClient VehicleClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DrogeVehicle? DrogeVehicle { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private readonly CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        if (IsNew == true || DrogeVehicle is null)
        {
            DrogeVehicle = new DrogeVehicle();
        }
    }

    private async Task Submit()
    {
        if (IsNew == true && DrogeVehicle is not null)
        {
            var newResult = await VehicleClient.PutVehicleAsync(DrogeVehicle, _cls.Token);
            if (newResult.NewId is not null)
                DrogeVehicle.Id = newResult.NewId.Value;
        }
        else
        {
            await VehicleClient.PatchVehicleAsync(DrogeVehicle, _cls.Token);
        }

        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}