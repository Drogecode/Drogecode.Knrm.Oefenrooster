using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Function;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages.Externals;

public partial class SharedActions : IDisposable
{
    [Inject] private IStringLocalizer<SharedActions> L { get; set; } = default!;
    [Inject, NotNull] private CustomStateProvider? AuthenticationStateProvider { get; set; }
    [Inject, NotNull] private IAuthenticationClient? AuthenticationClient { get; set; }
    [Inject, NotNull] private UserRepository? UserRepository { get; set; }
    [Inject, NotNull] private FunctionRepository? FunctionRepository { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }
    [Parameter] public Guid? Id { get; set; }
    private readonly CancellationTokenSource _cls = new();
    private List<DrogeFunction>? _functions;
    private bool _isLoaded;
    private bool _isAuthenticated;
    private bool _success;
    private MudTextField<string> _pwField;
    private MudForm _form;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
            DebugHelper.WriteLine($"_isAuthenticated `{_isAuthenticated}`");
            if (_isAuthenticated)
            {
                var isExternal = await UserHelper.InRole(AuthenticationState, AccessesNames.AUTH_External);
                DebugHelper.WriteLine($"isExternal `{isExternal}`");
                if (isExternal)
                {
                    var claims = authState.User;
                    var externalId = claims?.Identities.FirstOrDefault()!.Claims.FirstOrDefault(x => x.Type.Equals("ExternalId"))?.Value ?? string.Empty;
                    var parsedExternalId = Guid.TryParse(externalId, out var externalIdGuid);
                    if (!parsedExternalId || externalIdGuid != Id)
                    {
                        DebugHelper.WriteLine($"parsedExternalId: `{parsedExternalId}` externalId: `{externalId}`");
                        await AuthenticationStateProvider.Logout();
                        StateHasChanged();
                    }
                }
            }

            _isLoaded = true;
            StateHasChanged();
        }
    }

    private async Task ValidatePassword()
    {
        await _form.Validate();
        if (!_form.IsValid) return;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        _isAuthenticated = authState.User?.Identity?.IsAuthenticated ?? false;
        if (!_isAuthenticated)
        {
            var body = new AuthenticateExternalRequest(Id, _pwField.Value, DefaultSettingsHelper.CURRENT_VERSION);
            await AuthenticationClient.AuthenticateExternalAsync(body, _cls.Token);
            await AuthenticationStateProvider.loginCallback();
            StateHasChanged();
        }
    }

    public void Dispose()
    {
        _cls.Cancel();
    }
}