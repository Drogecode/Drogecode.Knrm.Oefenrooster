using Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public sealed partial class Alerts : IDisposable
{
    [Inject] private IStringLocalizer<Alerts> L { get; set; } = default!;
    [Inject] private PreComRepository _preComRepository { get; set; } = default!;
    private CancellationTokenSource _cls = new();
    private List<PreComAlert>? _alerts;
    protected override async Task OnInitializedAsync()
    {
        _alerts = await _preComRepository.GetAll(_cls.Token);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
