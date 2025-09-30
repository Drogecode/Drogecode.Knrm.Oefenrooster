using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class MonthItem : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<MonthItem>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IMonthItemClient? MonthItemClient { get; set; }
    [Inject, NotNull] private IDialogService? DialogProvider { get; set; }

    private GetMultipleMonthItemResponse? _monthItems;
    private CancellationTokenSource _cls = new();
    private RefreshModel _refreshModel = new();
    private int _currentPage = 1;
    private int _count = 30;
    private bool _bussy;
    private bool _includeExpired;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _monthItems = await MonthItemClient.GetAllItemsAsync(_count, 0, _includeExpired, _cls.Token);
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            StateHasChanged();
        }
    }

    private async Task Next(int nextPage)
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        var skip = (nextPage - 1) * _count;
        _monthItems = await MonthItemClient.GetAllItemsAsync(_count, skip, _includeExpired, _cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    private async Task IncludeExpiredChanged(bool value)
    {
        _includeExpired = value;
        _monthItems = await MonthItemClient.GetAllItemsAsync(_count, 0, _includeExpired, _cls.Token);
    }

    private async Task Delete(RoosterItemMonth? monthItem)
    {
        if (monthItem is null)
            return;
        var deleteResult = await MonthItemClient.DeleteMonthItemAsync(monthItem.Id, _cls.Token);
        if (deleteResult)
        {
            _monthItems!.MonthItems!.Remove(monthItem);
        }
    }

    private Task OpenMonthItemDialog(RoosterItemMonth? monthItem, bool isNew)
    {
        var parameters = new DialogParameters<MonthItemDialog>
        {
            { x => x.MonthItem, monthItem },
            { x => x.IsNew, isNew },
            { x => x.Refresh, _refreshModel },
        };
        var options = new DialogOptions() { FullScreen = true, FullWidth = true };
        return DialogProvider.ShowAsync<MonthItemDialog>(isNew ? L["Patch month item"] : L["Edit month item"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _monthItems = await MonthItemClient.GetAllItemsAsync(_count, 0, _includeExpired, _cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}