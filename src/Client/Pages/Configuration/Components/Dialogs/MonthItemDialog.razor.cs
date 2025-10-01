using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components.Dialogs;

public sealed partial class MonthItemDialog : IDisposable
{
    [Inject] private IStringLocalizer<MonthItemDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IMonthItemClient MonthItemClient { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public RoosterItemMonth? MonthItem { get; set; }
    [Parameter] public RefreshModel? Refresh { get; set; }
    [Parameter] public bool? IsNew { get; set; }

    private readonly CancellationTokenSource _cls = new();
    void Cancel() => MudDialog.Cancel();

    protected override void OnParametersSet()
    {
        if (IsNew == true || MonthItem is null)
        {
            MonthItem = new RoosterItemMonth
            {
                Month = 1,
            };
        }
    }

    private async Task Submit()
    {
        if (IsNew == true && MonthItem is not null)
        {
            var newResult = await MonthItemClient.PutItemAsync(MonthItem, _cls.Token);
            MonthItem.Id = newResult.NewId;
        }
        else
        {
            await MonthItemClient.PatchItemAsync(MonthItem, _cls.Token);
        }

        if (Refresh is not null) await Refresh.CallRequestRefreshAsync();
        MudDialog.Close(DialogResult.Ok(true));
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}