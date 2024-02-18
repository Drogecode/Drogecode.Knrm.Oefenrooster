using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DayItem : IDisposable
{
    [Inject] private IStringLocalizer<DayItem> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ICalendarItemClient _calendarItemClient { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private FunctionRepository _functionRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private RefreshModel _refreshModel = new();
    private GetMultipleDayItemResponse? _items;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private bool _bussy;
    private int _currentPage = 1;
    private int _count = 30;
    private int _skip = 0;

    protected override async Task OnParametersSetAsync()
    {
        _items = await _calendarItemClient.GetAllFutureDayItemsAsync(_count, _skip, true);
        _users = await _userRepository.GetAllUsersAsync(false, false, _cls.Token);
        _functions = await _functionRepository.GetAllFunctionsAsync();
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
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<DayItemDialog>(L["Edit day item"], parameters, options);
    }

    private async Task Next(int nextPage)
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        _skip = (nextPage - 1) * _count;
        _items = await _calendarItemClient.GetAllFutureDayItemsAsync(_count, _skip, true);
        _bussy = false;
        StateHasChanged();
    }

    private async Task Delete(RoosterItemDay? dayItem)
    {
        if (dayItem is null)
            return;
        var deleteResult = await _calendarItemClient.DeleteDayItemAsync(dayItem.Id, _cls.Token);
        if (deleteResult is true)
        {
            _items!.DayItems!.Remove(dayItem);
        }
    }

    private async Task RefreshMeAsync()
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _items = await _calendarItemClient.GetAllFutureDayItemsAsync(_count, _skip, true);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.CallRequestRefresh();
    }
}
