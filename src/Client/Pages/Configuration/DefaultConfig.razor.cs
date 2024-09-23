using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DefaultConfig : IDisposable
{
    [Inject] private IStringLocalizer<DefaultConfig> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private GetAllDefaultScheduleResponse? _defaults;
    private bool _bussy;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _defaults = await _defaultScheduleRepository.GetAllDefaultSchedule(_cls.Token);
            StateHasChanged();
        }
    }

    private async Task Delete(DefaultSchedule? defaultSchedule)
    {
        if (defaultSchedule is null)
            return;
        /*var deleteResult = await _defaultScheduleRepository.Delete(defaultSchedule.Id, _cls.Token);
        if (deleteResult is true)
        {
            _defaults!.DefaultSchedules!.Remove(defaultSchedule);
        }*/
    }

    private void OpenMonthItemDialog(DefaultSchedule? defaultSchedule, bool isNew)
    {
        //DayItemDialog
        /*var parameters = new DialogParameters<MonthItemDialog> {
            { x=> x.MonthItem, monthItem},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        DialogProvider.Show<MonthItemDialog>(isNew ? L["Patch month item"] : L["Edit month item"], parameters, options);*/
    }

    private async Task RefreshMeAsync()
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _defaults = await _defaultScheduleRepository.GetAllDefaultSchedule(_cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}