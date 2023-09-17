using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class TrainingsTab : IDisposable
{
    [Inject] private IStringLocalizer<TrainingsTab> L { get; set; } = default!;
    [Inject] private SharePointRepository _sharePointRepository { get; set; } = default!;
    private List<SharePointTraining>? _sharePointTrainings;
    private CancellationTokenSource _cls = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _sharePointTrainings = await _sharePointRepository.GetLastTrainingsForCurrentUser(10, 0, _cls.Token);
            StateHasChanged();
        }
    }
    public void Dispose()
    {
        _cls.Cancel();
    }
}
