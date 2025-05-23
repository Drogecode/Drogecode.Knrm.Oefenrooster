﻿using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner.Components;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;

public sealed partial class Audit : IDisposable
{
    [Inject] private IStringLocalizer<TrainingHistoryDialog> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IAuditClient AuditClient { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private UserRepository _userRepository { get; set; } = default!;
    [Inject] private ScheduleRepository _scheduleRepository { get; set; } = default!;
    [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
    [Parameter] public List<DrogeUser>? _users { get; set; }

    private GetTrainingAuditResponse? _trainingAudits = null;
    private readonly CancellationTokenSource _cls = new();
    private bool _busy;
    private int _currentPage = 1;
    private readonly int _count = 30;
    private int _skip;
    protected override async Task OnParametersSetAsync()
    {
        _trainingAudits = await AuditClient.GetAllTrainingsAuditAsync(_count, _skip, _cls.Token);
        _users = await _userRepository.GetAllUsersAsync(true, false, false, _cls.Token);
    }

    private async Task Next(int nextPage)
    {
        if (_busy) return;
        _busy = true;
        _trainingAudits = null;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        _skip = (nextPage - 1) * _count;
        _trainingAudits = await AuditClient.GetAllTrainingsAuditAsync(_count, _skip, _cls.Token);
        _busy = false;
        StateHasChanged();
    }

    private async Task Goto(Guid? trainingId)
    {
        if (trainingId is null) return;
        Navigation.NavigateTo($"/training/{trainingId}");
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
