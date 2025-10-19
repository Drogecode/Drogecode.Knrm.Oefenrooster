using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class TrainingsTab : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<TrainingsTab>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IStringLocalizer<DateToString>? LDateToString { get; set; } 
    [Inject, NotNull] private IReportTrainingClient? ReportTrainingClient { get; set; }
    [Inject, NotNull] private ReportTrainingRepository? ReportTrainingRepository { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public List<DrogeUser>? Users { get; set; }
    [Parameter] public List<DrogeFunction>? Functions { get; set; }
    [Parameter] public Guid? DrogeTrainingId { get; set; }
    private MultipleReportTrainingsResponse? _reportTrainings;
    private readonly CancellationTokenSource _cls = new();
    private IEnumerable<DrogeUser> _selectedUsersTraining = new List<DrogeUser>();
    private readonly List<string> _trainingTypes = [];
    private IEnumerable<string> _selectedTrainingTypes = new List<string>();
    private const int DEFAULT_COUNT = 10;
    private int _currentPage = 1;
    private int _count = DEFAULT_COUNT;
    private bool _busy;
    private bool _multiSelection;
    private bool _isTaco;

    protected override async Task OnParametersSetAsync()
    {
        _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_super_user);
        _multiSelection = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_training_history_full);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await UpdateReportTrainings(0);
            var trainingTypes = await ReportTrainingRepository.Distinct(DistinctReport.Type, _cls.Token);
            if (trainingTypes?.Values is not null)
            {
                foreach (var value in trainingTypes.Values.Where(x => x is not null))
                {
                    _trainingTypes.Add(value!);
                }
            }

            StateHasChanged();
        }
    }

    private async Task UpdateReportTrainings(int skip)
    {
        if (DrogeTrainingId is not null)
        {
            _reportTrainings = await ReportTrainingClient.GetReportsLinkedToTrainingAsync(DrogeTrainingId.Value, _count, skip, _cls.Token);
        }
        else
        {
            _reportTrainings = await ReportTrainingRepository.GetLastTraining(_selectedUsersTraining, _selectedTrainingTypes, _count, skip, _cls.Token);
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _selectedUsersTraining = selection;
        _count = DEFAULT_COUNT;
        await UpdateReportTrainings(0);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task SelectedTrainingChanged(IEnumerable<string> selection)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _selectedTrainingTypes = selection;
        _count = DEFAULT_COUNT;
        await UpdateReportTrainings(0);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task OnCountChange(int arg)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _count = arg;
        await UpdateReportTrainings(0);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task Next(int nextPage)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        if (nextPage <= 0) return;
        _currentPage = nextPage;
        var skip = (nextPage - 1) * _count;
        await UpdateReportTrainings(skip);
        _busy = false;
        StateHasChanged();
    }

    private bool IsExpanded(DrogeTraining arg)
    {
        return _reportTrainings?.TotalCount == 1;
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}