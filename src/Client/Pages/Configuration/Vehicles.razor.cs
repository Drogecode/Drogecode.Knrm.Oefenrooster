using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Vehicle;
using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class Vehicles : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<Vehicles>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IVehicleClient? VehicleClient { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private readonly RefreshModel _refreshModel = new();
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

    private Task OpenVehicleDialog(DrogeVehicle? vehicle, bool isNew)
    {
        var parameters = new DialogParameters<VehicleDialog>
        {
            { x => x.DrogeVehicle, vehicle },
            { x => x.IsNew, isNew },
            { x => x.Refresh, _refreshModel },
        };
        var options = new DialogOptions() { FullScreen = true, FullWidth = true };
        return DialogProvider.ShowAsync<VehicleDialog>(isNew ? L["Put vehicle"] : L["Edit vehicle"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _vehicles = await VehicleClient.GetAllAsync(false, _cls.Token);
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
        _cls.Cancel();
    }
}