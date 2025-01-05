using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DayItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DayItem : IDisposable
{
    [Inject] private IStringLocalizer<DayItem> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IDayItemClient DayItemClient { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;

    private CancellationTokenSource _cls = new();
    private RefreshModel _refreshModel = new();
    private GetMultipleDayItemResponse? _items;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private bool _busy;
    private int _currentPage = 1;
    private int _count = 30;
    private int _skip;

    protected override async Task OnParametersSetAsync()
    {
        _items = await DayItemClient.GetAllFutureAsync(_count, _skip, true, false);
        _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
        _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private void OpenDayItemDialog(RoosterItemDay? dayItem, bool isNew)
    {
        //DayItemDialog
        var parameters = new DialogParameters<DayItemDialog> {
            { x=> x.DayItem, dayItem},
            { x=> x.IsNew, isNew},
            { x=> x.Users, _users },
            { x=> x.Functions, _functions },
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions() { FullScreen = true, FullWidth = true };
        DialogProvider.Show<DayItemDialog>(L["Edit day item"], parameters, options);
    }

    private async Task Next(int nextPage)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        _skip = (nextPage - 1) * _count;
        _items = await DayItemClient.GetAllFutureAsync(_count, _skip, true, false);
        _busy = false;
        StateHasChanged();
    }

    private async Task Delete(RoosterItemDay? dayItem)
    {
        if (dayItem is null)
            return;
        var deleteResult = await DayItemClient.DeleteDayItemAsync(dayItem.Id, _cls.Token);
        if (deleteResult)
        {
            _items!.DayItems!.Remove(dayItem);
        }
    }

    private async Task RefreshMeAsync()
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _items = await DayItemClient.GetAllFutureAsync(_count, _skip, true, false);
        _busy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.CallRequestRefresh();
    }
}
