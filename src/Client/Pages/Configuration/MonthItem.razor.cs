using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class MonthItem : IDisposable
{
    [Inject] private IStringLocalizer<MonthItem> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IMonthItemClient MonthItemClient { get; set; } = default!;

    private GetMultipleMonthItemResponse? _monthItems;
    private CancellationTokenSource _cls = new();
    private int _currentPage = 1;
    private int _count = 30;
    private bool _bussy;

    protected override async Task OnParametersSetAsync()
    {
        _monthItems = await MonthItemClient.GetAllItemsAsync(_count, 0, false, _cls.Token);
    }

    private async Task Next(int nextPage)
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        var skip = (nextPage - 1) * _count;
        _monthItems = await MonthItemClient.GetAllItemsAsync(_count, skip, false, _cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
