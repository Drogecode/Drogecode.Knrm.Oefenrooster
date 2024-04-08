using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;
using static MudBlazor.CategoryTypes;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class Audit : IDisposable
{
    [Inject] private IStringLocalizer<TrainingHistoryDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IAuditClient AuditClient { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] MudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public List<DrogeUser>? _users { get; set; }

    private GetTrainingAuditResponse? _trainingAudits = null;
    private CancellationTokenSource _cls = new();
    private bool _bussy;
    private int _currentPage = 1;
    private int _count = 30;
    private int _skip = 0;
    protected override async Task OnParametersSetAsync()
    {
        _trainingAudits = await AuditClient.GetAllTrainingsAuditAsync(_count, _skip, _cls.Token);
        _users = await _userRepository.GetAllUsersAsync(true, false, false, _cls.Token);
    }

    private async Task Next(int nextPage)
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        _skip = (nextPage - 1) * _count;
        _trainingAudits = await AuditClient.GetAllTrainingsAuditAsync(_count, _skip, _cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
