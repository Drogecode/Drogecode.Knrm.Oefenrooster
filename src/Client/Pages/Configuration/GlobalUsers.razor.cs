using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class GlobalUsers : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<GlobalUsers>? L { get; set; }
    [Inject, NotNull] private IUserGlobalClient? UserGlobalClient { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }
    
    private readonly CancellationTokenSource _cls = new();

    private AllDrogeUserGlobalResponse? _globalUsers;
    private RefreshModel _refreshModel = new();
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            _globalUsers = await UserGlobalClient.GetAllAsync();
            StateHasChanged();
        }
    }
    
    private Task OpenGlobalUserDialog(DrogeUserGlobal? globalUser, bool isNew)
    {
        var parameters = new DialogParameters<AddGlobalUserDialog>
        {
            { x => x.GlobalUser, globalUser },
            { x => x.IsNew, isNew },
            { x => x.Refresh, _refreshModel },
        };
        var options = new DialogOptions()
        {
            MaxWidth = MaxWidth.Large,
            CloseButton = true,
            FullWidth = true
        };
        return DialogProvider.ShowAsync<AddGlobalUserDialog>(isNew ? L["Put global user"] : L["Edit global user"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        _globalUsers = await UserGlobalClient.GetAllAsync();
        StateHasChanged();
    }

    public void Dispose()
    {
        _refreshModel.RefreshRequestedAsync -= RefreshMeAsync;
        _cls.Cancel();
    }
}