using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public partial class Vehicle : IDisposable
{
    [Inject] private IStringLocalizer<Vehicle> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IVehicleClient VehicleClient { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private RefreshModel _refreshModel = new();
    private MultipleVehicleResponse? _vehicles;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _vehicles = await VehicleClient.GetAllAsync(false, _cls.Token);
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            StateHasChanged();
        }
    }

    private void OpenMonthItemDialog(DrogeVehicle? vehicle, bool isNew)
    {
        var parameters = new DialogParameters<VehicleDialog> {
            { x=> x.DrogeVehicle, vehicle},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        DialogProvider.Show<VehicleDialog>(isNew ? L["Put vehicle"] : L["Edit vehicle"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _vehicles = await VehicleClient.GetAllAsync(false, _cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}