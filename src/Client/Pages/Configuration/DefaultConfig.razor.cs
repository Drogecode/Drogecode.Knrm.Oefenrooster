using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DefaultConfig : IDisposable
{
    [Inject] private IStringLocalizer<DefaultConfig> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IStringLocalizer<DateToString> LDateToString { get; set; } = default!;
    [Inject] private IDialogService DialogProvider { get; set; } = default!;
    [Inject] private DefaultScheduleRepository DefaultScheduleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private GetAllDefaultScheduleResponse? _defaults;
    private RefreshModel _refreshModel = new();
    private bool _bussy;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _defaults = await DefaultScheduleRepository.GetAllDefaultSchedule(_cls.Token);
            _refreshModel.RefreshRequestedAsync += RefreshMeAsync;
            StateHasChanged();
        }
    }

    private void OpenMonthItemDialog(DefaultSchedule? defaultSchedule, bool isNew)
    {
        var parameters = new DialogParameters<DefaultConfigDialog> {
            { x=> x.DefaultSchedule, defaultSchedule},
            { x=> x.IsNew, isNew},
            { x=> x.Refresh, _refreshModel },
        };
        DialogOptions options = new DialogOptions() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        DialogProvider.Show<DefaultConfigDialog>(isNew ? L["Patch default schedule"] : L["Edit default schedule"], parameters, options);
    }

    private async Task RefreshMeAsync()
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _defaults = await DefaultScheduleRepository.GetAllDefaultSchedule(_cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}