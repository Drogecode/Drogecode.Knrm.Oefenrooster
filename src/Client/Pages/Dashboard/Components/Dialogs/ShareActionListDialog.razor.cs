using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportActionShared;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components.Dialogs;

public partial class ShareActionListDialog : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ShareActionDialog>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IStringLocalizer<DateToString>? LDateToString { get; set; }
    [Inject, NotNull] private IReportActionSharedClient? ReportActionSharedClient { get; set; }
    [CascadingParameter, NotNull] IMudDialogInstance? MudDialog { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }

    private readonly CancellationTokenSource _cls = new();
    private MultipleReportActionShareConfigurationResponse? _configurationResponse = null;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _configurationResponse = await ReportActionSharedClient.GetAllReportActionSharedAsync(_cls.Token);
            StateHasChanged();
        }
    }

    private async Task Delete(ReportActionSharedConfiguration? contextItem)
    {
        if (contextItem is null)
            return;
        var delete = await ReportActionSharedClient.DeleteReportActionSharedAsync(contextItem.Id, _cls.Token);
        if (delete is not null && delete.Success)
        {
            _configurationResponse = await ReportActionSharedClient.GetAllReportActionSharedAsync(_cls.Token);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}