﻿using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Security.Claims;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

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

    private CurrentUser _currentUser = new () {
        IsAuthenticated = true,
        Id = Guid.CreateVersion7(),
        UserName = "TEST USER",
        Claims = new List<KeyValuePair<string, string>>
        {
            new ("http://schemes.random.net/identity/upn", "TEST USER"),
            new ("ValidFrom", DateTime.Now.AddHours(-1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new ("ValidTo", DateTime.Now.AddHours(1).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")),
            new (ClaimTypes.Role, AccessesNames.AUTH_scheduler_description_read)
        }
    };
    
    public async Task<CurrentUser> CurrentUserInfoAsync()
    {
        return _currentUser;
    }

    public async Task<CurrentUser> CurrentUserInfoAsync(CancellationToken cancellationToken)
    {
        return await CurrentUserInfoAsync();
    }

    public async Task<bool> SwitchUserAsync(SwitchUserRequest body)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SwitchUserAsync(SwitchUserRequest body, CancellationToken cancellationToken)
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

    public async Task<RefreshResponse> RefreshUserAsync()
    {
        return new RefreshResponse()
        {
            Success = true,
            State = RefreshState.AuthenticationRefreshed,
        };
    }

    public async Task<RefreshResponse> RefreshUserAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> RefreshAsync()
    {
        return true;
    }

    public async Task<bool> RefreshAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}