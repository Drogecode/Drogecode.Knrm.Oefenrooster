using Drogecode.Knrm.Oefenrooster.Client.Components.DrogeCode;
using Drogecode.Knrm.Oefenrooster.Client.Repositories;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using Microsoft.Extensions.Localization;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Dashboard.Components;

public sealed partial class TrainingsTab : IDisposable
{
    [Inject] private IStringLocalizer<TrainingsTab> L { get; set; } = default!;
    [Inject] private IStringLocalizer<DateToString> LDateToString { get; set; } = default!;
    [Inject] private SharePointRepository SharePointRepository { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] [EditorRequired] public DrogeUser User { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeUser> Users { get; set; } = default!;
    [Parameter] [EditorRequired] public List<DrogeFunction> Functions { get; set; } = default!;
    private MultipleSharePointTrainingsResponse? _sharePointTrainings;
    private CancellationTokenSource _cls = new();
    private IEnumerable<DrogeUser> _selectedUsersTraining = new List<DrogeUser>();
    private static readonly List<string> _trainingDifferentStart = ["HRB Evenement", "KNRM Evenement", "KNRM Onderhoud"];
    private int _currentPage = 1;
    private int _count = 10;
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
            _sharePointTrainings = await SharePointRepository.GetLastTrainingsForCurrentUser(10, 0, _cls.Token);
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
        _sharePointTrainings = await SharePointRepository.GetLastTrainings(selection, 10, 0, _cls.Token);
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
        _sharePointTrainings = await SharePointRepository.GetLastTrainings(_selectedUsersTraining, _count, skip, _cls.Token);
        _busy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}