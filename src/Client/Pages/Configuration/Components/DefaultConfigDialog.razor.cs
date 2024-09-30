using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;

public sealed partial class DefaultConfigDialog: IDisposable
{
    [Inject] private IStringLocalizer<MonthItemDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository DefaultScheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public DefaultSchedule? DefaultSchedule { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        if (IsNew == true || DefaultSchedule is null)
        {
            DefaultSchedule = new DefaultSchedule
            {
            };
        }
    }

    private async Task Submit()
    {
        /*if (IsNew == true && DefaultSchedule is not null)
        {
            var newResult = await DefaultScheduleRepository.put(MonthItem, _cls.Token);
            MonthItem.Id = newResult.NewId;
        }
        else
        {
            var patchResult = await DefaultScheduleRepository.PatchItemAsync(MonthItem, _cls.Token);
        }*/

        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}