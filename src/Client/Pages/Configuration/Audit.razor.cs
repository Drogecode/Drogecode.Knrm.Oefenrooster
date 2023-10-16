using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

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

    private List<TrainingAudit>? _trainingAudits = null;
    private CancellationTokenSource _cls = new();
    protected override async Task OnParametersSetAsync()
    {
        _trainingAudits = (await AuditClient.GetAllTrainingsAuditAsync()).TrainingAudits;
        _users = await _userRepository.GetAllUsersAsync(true);
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
