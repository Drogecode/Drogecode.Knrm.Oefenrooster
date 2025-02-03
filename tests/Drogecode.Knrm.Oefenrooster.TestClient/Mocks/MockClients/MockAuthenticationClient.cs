using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.TestClient.Mocks.MockClients;

public class MockAuthenticationClient : IAuthenticationClient
{
    public async Task<GetLoginSecretsResponse> GetLoginSecretsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<GetLoginSecretsResponse> GetLoginSecretsAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateUserGetAsync(string code, string state, string sessionState, string redirectUrl)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateUserGetAsync(string code, string state, string sessionState, string redirectUrl, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateUserAsync(AuthenticateRequest body)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateUserAsync(AuthenticateRequest body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateExternalAsync(AuthenticateExternalRequest body)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateExternalAsync(AuthenticateExternalRequest body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> GetAuthenticateDirectEnabledAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> GetAuthenticateDirectEnabledAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateDirectAsync(AuthenticateDirectRequest body)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AuthenticateDirectAsync(AuthenticateDirectRequest body, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<CurrentUser> CurrentUserInfoAsync()
    {
        return new CurrentUser();
    }

    public async Task<CurrentUser> CurrentUserInfoAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync()
    {
        throw new NotImplementedException();
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RefreshAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RefreshAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}