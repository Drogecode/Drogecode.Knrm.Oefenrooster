using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class DefaultSchedule : IDisposable
{
    [Inject] private IStringLocalizer<DefaultSchedule> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private DefaultScheduleRepository _defaultScheduleRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private GetAllDefaultScheduleResponse? _defaults;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _defaults = await _defaultScheduleRepository.GetAllDefaultSchedule(_cls.Token);
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}