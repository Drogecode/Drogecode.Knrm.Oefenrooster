using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

public partial class AddGlobalUserDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<AddGlobalUserDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IUserGlobalClient? UserGlobalClient { get; set; }
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public DrogeUserGlobal? GlobalUser { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    [AllowNull] private MudForm _form;
    private readonly CancellationTokenSource _cls = new();
    private bool _success;
    private string[] _errors = [];

    void Cancel() => MudDialog.Cancel();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && IsNew == true && GlobalUser is null)
        {
            GlobalUser = new DrogeUserGlobal();
        }
    }

    private async Task Submit()
    {
        if (GlobalUser is null)
            return;
        if (IsNew == true)
        {
            var result = await UserGlobalClient.PutGlobalUserAsync(GlobalUser, _cls.Token);
            if (result.Success && result.NewId is not null)
            {
                IsNew = false;
                GlobalUser.Id = result.NewId.Value;
                if (Refresh is not null)
                    await Refresh.CallRequestRefreshAsync();
                MudDialog.Close();
            }
        }
        else
        {
            var patchResult = await UserGlobalClient.PatchGlobalUserAsync(GlobalUser, _cls.Token);
            if (patchResult.Success)
            {
                if (Refresh is not null)
                    await Refresh.CallRequestRefreshAsync();
                MudDialog.Close();
            }
        }

        MudDialog.Close();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}