using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportTraining;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class TrainingsTab : IDisposable
{
    [Inject] private IStringLocalizer<TrainingsTab> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IStringLocalizer<DateToString> LDateToString { get; set; } = default!;
    [Inject] private ReportTrainingRepository ReportTrainingRepository { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private MultipleReportTrainingsResponse? _reportTrainings;
    private CancellationTokenSource _cls = new();
    private IEnumerable<DrogeUser> _selectedUsersTraining = new List<DrogeUser>();
    private List<string> _trainingTypes = [];
    private IEnumerable<string> _selectedTrainingTypes = new List<string>();
    private const int DEFAULT_COUNT = 10;
    private int _currentPage = 1;
    private int _count = DEFAULT_COUNT;
    private bool _busy;
    private bool _multiSelection;
    private bool _isTaco;

    protected override async Task OnParametersSetAsync()
    {
        _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_Taco);
        _multiSelection = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_training_history_full);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _reportTrainings = await ReportTrainingRepository.GetLastTrainingsForCurrentUser(10, 0, _cls.Token);
            var trainingTypes = await ReportTrainingRepository.Distinct(DistinctReport.Type, _cls.Token);
            if (trainingTypes?.Values is not null)
            {
                foreach (var value in trainingTypes.Values.Where(x=> x is not null))
                {
                    _trainingTypes.Add(value!);
                }
            }
            var thisUser = Users.FirstOrDefault(x => x.Id == User.Id);
            if (thisUser is not null)
            {
                ((List<DrogeUser>)_selectedUsersTraining).Add(thisUser);
            }

            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _selectedUsersTraining = selection;
        _count = DEFAULT_COUNT;
        _reportTrainings = await ReportTrainingRepository.GetLastTraining(selection, _selectedTrainingTypes, 10, 0, _cls.Token);
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
        _reportTrainings = await ReportTrainingRepository.GetLastTraining(_selectedUsersTraining, _selectedTrainingTypes, _count, 0, _cls.Token);
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
        _reportTrainings = await ReportTrainingRepository.GetLastTraining(_selectedUsersTraining, _selectedTrainingTypes, _count, 0, _cls.Token);
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
        _reportTrainings = await ReportTrainingRepository.GetLastTraining(_selectedUsersTraining, _selectedTrainingTypes, _count, skip, _cls.Token);
        _busy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}