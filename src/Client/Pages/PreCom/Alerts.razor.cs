using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.PreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Text.Json;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.PreCom;

public sealed partial class Alerts : IDisposable
{
    [Inject] private IStringLocalizer<Alerts> L { get; set; } = default!;
    [Inject] private PreComRepository PreComRepository { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private UserRepository UserRepository { get; set; } = default!;
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    private CancellationTokenSource _cls = new();
    private MultiplePreComAlertsResponse? _alerts;
    private DrogeUser? _user;
    private bool _isTaco;
    private bool _showHowTo;
    private int _currentPage = 1;
    private int _count = 30;
    private bool _bussy;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _alerts = await PreComRepository.GetAllAlerts(_count, 0, _cls.Token);
            _user = await UserRepository.GetCurrentUserAsync(_cls.Token);
            _isTaco = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_super_user);
            StateHasChanged();
        }
    }

    public static string JsonPrettify(string? json)
    {
        if ( string.IsNullOrEmpty(json))
        {
            return string.Empty;
        }
        using var jDoc = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(jDoc, new JsonSerializerOptions { WriteIndented = true });
    }

    private async Task Next(int nextPage)
    {
        if (_bussy) return;
        _bussy = true;
        StateHasChanged();
        _currentPage = nextPage;
        if (nextPage <= 0) return;
        var skip = (nextPage - 1) * _count;
        _alerts = await PreComRepository.GetAllAlerts(_count, skip, _cls.Token);
        _bussy = false;
        StateHasChanged();
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}
