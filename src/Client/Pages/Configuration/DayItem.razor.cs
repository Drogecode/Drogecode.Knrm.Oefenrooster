using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DayItem : IDisposable
{
    [Inject] private IStringLocalizer<DayItem> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private ICalendarItemClient _calendarItemClient { get; set; } = default!;
    [Inject] private IDialogService _dialogProvider { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private RefreshModel _refreshModel = new();
    private List<RoosterItemDay>? _items;

    protected override async Task OnParametersSetAsync()
    {
        _items =  (await _calendarItemClient.GetAllFutureDayItemsAsync(30, 0))?.DayItems;
        _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
    }

    private void OpenDayItemDialog(RoosterItemDay? dayItem, bool isNew)
    {
        //DayItemDialog
        var parameters = new DialogParameters<DayItemDialog> {
            { x=> x.DayItem, dayItem},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        _dialogProvider.Show<DayItemDialog>(L["Edit day item"], parameters, options);
    }

    private async Task Delete(RoosterItemDay? dayItem)
    {
        throw new NotImplementedException("ToDo");
        /*var response = await _calendarItemClient.Delete(dayItem!.Id, _cls.Token);
        if (response != null && response.Success)
        {
            await RefreshMeAsync();
        }*/
    }

    private async Task RefreshMeAsync()
    {
        _items = (await _calendarItemClient.GetAllFutureDayItemsAsync(30, 0))?.DayItems;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
        _refreshModel.CallRequestRefresh();
    }
}
