using Drogecode.Knrm.Oefenrooster.Client.Models;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Configuration;
using Drogecode.Knrm.Oefenrooster.Client.Pages.Planner;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class ActionsTab : IDisposable
{
    [Inject] private IStringLocalizer<ActionsTab> L { get; set; } = default!;
    [Inject] private SharePointRepository _sharePointRepository { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter][EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter][EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private MultipleSharePointActionsResponse? _sharePointActions;
    private CancellationTokenSource _cls = new();
    private IEnumerable<DrogeUser> _selectedUsersAction = new List<DrogeUser>();
    private int _currentPage = 1;
    private int _count = 10;
    private bool _bussy;
    private bool _multiSelection;
    protected override async Task OnParametersSetAsync()
    {
        if (AuthenticationState is not null)
        {
            var authState = await AuthenticationState;
            var user = authState?.User;
            if (user is not null)
            {
                _multiSelection = user.IsInRole(AccessesNames.AUTH_action_history_full);
            }
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _sharePointActions = await _sharePointRepository.GetLastActionsForCurrentUser(10, 0, _cls.Token);
            if (Users is not null)
            {
                var thisUser = Users!.FirstOrDefault(x => x.Id == User.Id);
                if (thisUser is not null)
                {
                    ((List<DrogeUser>)_selectedUsersAction).Add(thisUser);
                }
            }
            StateHasChanged();
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        _bussy = true;
        StateHasChanged();
        _selectedUsersAction = selection;
        _sharePointActions = await _sharePointRepository.GetLastActions(selection, 10, 0, _cls.Token);
        _currentPage = 1;
        _bussy = false;
        StateHasChanged();
    }

    private async Task Next(int nextPage)
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        var skip = (nextPage - 1) * _count;
        _sharePointActions = await _sharePointRepository.GetLastActions(_selectedUsersAction, _count, skip, _cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
