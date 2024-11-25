using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public partial class SendAlertToForward : IDisposable
{
    [Inject] private IStringLocalizer<SendAlertToForward> L { get; set; } = default!;
    [Inject] private PreComRepository PreComRepository { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [Inject] private FunctionRepository FunctionRepository { get; set; } = default!;
    
    private CancellationTokenSource _cls = new();
    private PreComForward? _selectedForward;
    private string? _message;
    private bool _isSelectingUser;
    private bool _isSending;
    private List<DrogeUser>? _users;
    private List<DrogeFunction>? _functions;
    private IEnumerable<DrogeUser> _selectedUsers = new List<DrogeUser>();
    private List<PreComForward>? _forwards;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _users = await UserRepository.GetAllUsersAsync(false, false, false, _cls.Token);
            _functions = await FunctionRepository.GetAllFunctionsAsync(false, _cls.Token);
         StateHasChanged();   
        }
    }

    private async Task OnSelectionChanged(IEnumerable<DrogeUser> selection)
    {
        if (_isSelectingUser) return;
        _isSelectingUser = true;
        _forwards = null;
        _selectedForward = null;
        var selectionAsList = selection.ToList();
        _selectedUsers = selectionAsList;
        if (selectionAsList.Any() == true)
        {
            _forwards = (await PreComRepository.AllForwardsForUserAsync(selectionAsList.FirstOrDefault()!.Id, 10, 0, _cls.Token))?.PreComForwards;
        }

        _isSelectingUser = false;
        StateHasChanged();
    }

    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_message) || _selectedForward is null || _isSending) return;
        _isSending = true;
        StateHasChanged();
        var body = new PostForwardRequest
        {
            ForwardId = _selectedForward.Id,
            Message = _message,
        };
        await PreComRepository.PostForwardAsync(body, _cls.Token);
        _forwards = null;
        _selectedForward = null;
        _selectedUsers = [];
        _isSending = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}