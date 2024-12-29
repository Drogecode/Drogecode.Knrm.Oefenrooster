using System.Diagnostics.CodeAnalysis;
using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.ReportAction;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class ActionsTab : IDisposable
{
    [Inject, NotNull] private IStringLocalizer<ActionsTab>? L { get; set; }
    [Inject, NotNull] private IStringLocalizer<App>? LApp { get; set; }
    [Inject, NotNull] private IStringLocalizer<DateToString>? LDateToString { get; set; }
    [Inject, NotNull] private ReportActionRepository? ReportActionRepository { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public DrogeUser? User { get; set; }
    [Parameter] public Guid? SharedId { get; set; }
    [Parameter, EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter, EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    [Parameter, EditorRequired] public bool EnableOptions { get; set; }
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
    private string? _search;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _multiSelection = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_action_history_full);
            _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_super_user);
            var actionTypes = await ReportActionRepository.Distinct(DistinctReport.Type, _cls.Token);
            if (actionTypes?.Values is not null)
            {
                foreach (var value in actionTypes.Values.Where(x => x is not null))
                {
                    _actionTypes.Add(value!);
                }
            }

            if (User is not null)
            {
                var thisUser = Users.FirstOrDefault(x => x.Id == User.Id);
                if (thisUser is not null)
                {
                    ((List<DrogeUser>)_selectedUsersAction).Add(thisUser);
                }
            }
            _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _search, _count, 0, _cls.Token);

            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        if (_busy || !EnableOptions) return;
        _busy = true;
        StateHasChanged();
        _selectedUsersAction = selection;
        _count = DEFAULT_COUNT;
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _search, _count, 0, _cls.Token);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task SelectedActionChanged(IEnumerable<string> selection)
    {
        if (_busy || !EnableOptions) return;
        _busy = true;
        StateHasChanged();
        _selectedActionTypes = selection;
        _count = DEFAULT_COUNT;
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _search, _count, 0, _cls.Token);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task OnCountChange(int newCount)
    {
        if (_busy) return;
        _busy = true;
        StateHasChanged();
        _count = newCount;
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _search, _count, 0, _cls.Token);
        _currentPage = 1;
        _busy = false;
        StateHasChanged();
    }

    private async Task SearchChanged(string? search)
    {
        if (_busy || !EnableOptions) return;
        _busy = true;
        try
        {
            StateHasChanged();
            _search = search;
            _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _search, _count, 0, _cls.Token);
        }
        catch (Exception ex)
        {
            DebugHelper.WriteLine(ex);
        }

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
        _reportActions = await ReportActionRepository.GetLastActions(_selectedUsersAction, _selectedActionTypes, _search, _count, skip, _cls.Token);
        _busy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}