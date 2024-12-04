using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class ActionsTab : IDisposable
{
    [Inject] private IStringLocalizer<ActionsTab> L { get; set; } = default!;
    [Inject] private IStringLocalizer<App> LApp { get; set; } = default!;
    [Inject] private IStringLocalizer<DateToString> LDateToString { get; set; } = default!;
    [Inject] private ReportActionRepository ReportActionRepository { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private MultipleReportActionsResponse? _reportActions;
    private CancellationTokenSource _cls = new();
    private IEnumerable<DrogeUser> _selectedUsersAction = new List<DrogeUser>();
    private List<string> _actionTypes = [];
    private IEnumerable<string> _selectedActionTypes = new List<string>();
    private const int DEFAULT_COUNT = 10;
    private int _currentPage = 1;
    private int _count = DEFAULT_COUNT;
    private bool _busy;
    private bool _multiSelection;
    private bool _isTaco;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _multiSelection = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_action_history_full);
            _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_Taco);
            _reportActions = await ReportActionRepository.GetLastActionsForCurrentUser(_count, 0, _cls.Token);
            var actionTypes = await ReportActionRepository.Distinct(DistinctReport.Type, _cls.Token);
            if (actionTypes?.Values is not null)
            {
                foreach (var value in actionTypes.Values.Where(x=> x is not null))
                {
                    _actionTypes.Add(value!);
                }
            }
            var thisUser = Users.FirstOrDefault(x => x.Id == User.Id);
            if (thisUser is not null)
            {
                ((List<DrogeUser>)_selectedUsersAction).Add(thisUser);
            }
            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _selectedUsersAction = selection;
        _count = DEFAULT_COUNT;
        _reportActions = await ReportActionRepository.GetLastActions(selection, _selectedActionTypes, _count, 0, _cls.Token);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task SelectedActionChanged(IEnumerable<string> selection)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _selectedActionTypes = selection;
        _count = DEFAULT_COUNT;
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _count, 0, _cls.Token);
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
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _count, 0, _cls.Token);
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
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _count, skip, _cls.Token);
        _busy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}