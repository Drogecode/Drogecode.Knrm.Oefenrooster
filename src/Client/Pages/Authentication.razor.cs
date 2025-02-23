using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;
using Drogecode.Knrm.Oefenrooster.Shared.Enums;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Audit;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Diagnostics.CodeAnalysis;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;

public sealed partial class Authentication
{
    [Inject, NotNull] private NavigationManager? Navigation { get; set; }
    [Inject, NotNull] private IAuthenticationClient? AuthenticationClient { get; set; }
    [Inject, NotNull] private AuthenticationRepository? AuthenticationRepository { get; set; }
    [Inject, NotNull] private CustomStateProvider? AuthenticationStateProvider { get; set; }
    [Inject, NotNull] private IAuditClient? AuditClient { get; set; }
    [Parameter] public string? Action { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? code { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? state { get; set; }
    [Parameter, SupplyParameterFromQuery] public string? session_state { get; set; }
    private bool _showLogin;

    //https://learn.microsoft.com/nl-nl/azure/active-directory/develop/v2-protocols-oidc
    //https://codewithmukesh.com/blog/authentication-in-blazor-webassembly/
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var redirectUrl = $"{Navigation.BaseUri}authentication/login-callback";
            var directAuthenticationEnabled = await AuthenticationRepository.GetAuthenticateDirectEnabled();
            switch (Action)
            {
                case "login":
                    if (directAuthenticationEnabled)
                    {
                        _showLogin = true;
                        StateHasChanged();
                    }
                    else
                    {
                        await Login(redirectUrl);
                    }

                    break;
                case "login-callback":
                    await LoginCallback(redirectUrl);
                    break;
                case "logout":
                    await Logout();
                    break;
            }
        }
    }

    private async Task Login(string redirectUrl)
    {
        var secrets = await AuthenticationClient.GetLoginSecretsAsync();
        switch (secrets.IdentityProvider)
        {
            case IdentityProvider.Azure:
                await LoginAzure(secrets, redirectUrl);
                break;
            case IdentityProvider.KeyCloak:
                await LoginKeyCloak(secrets, redirectUrl);
                break;
        }
    }

    private async Task LoginCallback(string redirectUrl)
    {
        //https://localhost:44327/authentication/login-callback?
        //code=0.ATAAVUd12VSwnEqnf9pCpACTZa57pBzB1l5Jm5DDKyRP3eIwAFY.AgABAAIAAAAtyolDObpQQ5VtlI4uGjEPAgDs_wUA9P_0knpzIDaOclMFbZVWvzgkrOGgoKDWAkpTpALiDDTLmgIywpb0VXuMvskBVO3cfWNqd9ROR21fGh706ObeExSAitcGOiy3baOutXSpHfXKTfQ2rGtWuKSxTwdq7YacQ7vs9VKvMjCd1nCXkmG8v_5rJfBDHGlFgGJOrwQ79gnOmOjMT3OLFpoLBswNglns6Y09wmVE_bkEmoB2e7-vbDavshWK0KyUkz6uglcolkCVATAefabAEyt88nOuJRqi9F8tPBmeIznnu3FHKtMMolNoTH4il15oaav7VNvc4ea129_qxixCVcrm6SVvCYkRjKnIr0Rf2sog-MgHp034aCgcbhawUTYe9X1jsvUc7I9MQYjY811vSsMECvO0uzjaZapSkRWFhoc3i1KpyNjpOqn6UkvB7UUe-CzXPaxm4VaordKmEUjG73bp8-sEK1x5KOQ-zhF-DsmVOtR0WToItAC93qJryadYFHngEj3tCqv8oFiCiv2_tWOgkoFeECaKGeFIQx0FUOtcaWfz9Fub4CJfVH6yQOoL-QcyjGnbWfvYx2fRZE8gfy4mmpSBcZafbcf-R3IXMvvnhz1NZksLeZpYrwFPvljVNiVa3n4oU-3gBUZvcccmJxDcquPC_oSdOTnlsX9gfgGQvh8NJLnYa97now27rkvryhhwEbdlGi4NJJrXpAPYVs424RqWgST8B-QmVwohI5tKbfuyiSoM4kOXxY1xZuxano3HbU9mPwcZgh794JfLo1f7SLuLJhRMhH5f-jVsFlwU6vrJb9_GIDbP-kRZm9qkyWvvUgsTx8Tmdkobd5HR9priYAyX6K2fdQF4_yuYZyojCce-90K7CntvSdl4lEq0m8rhgLN7cR4u1ZtHidtyxci0p41Y8smhKMbLPLbyIsBGx7pe55Q
        //&state=WIAge8uGpFQ5RTNLZ9gEy9r0gAB6OgFJBiJOZ9Sn9TomA8a6QXABWfcrv51T3HG3
        //&session_state=524653cd-d523-4038-a704-6e58bb555568
        var result = await AuthenticationClient.AuthenticateUserAsync(new AuthenticateRequest( code, state, session_state, redirectUrl, DefaultSettingsHelper.CURRENT_VERSION));
        if (result)
        {
            await AuthenticationStateProvider.loginCallback();
            Navigation.NavigateTo("/");
        }
        else
        {
            Navigation.NavigateTo("/landing_page");
        }
    }

    private async Task Logout()
    {
        await AuditClient.PostLogAsync(new PostLogRequest { Message = "Logout start" });
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var logoutHint = authState.User.FindFirst(c => c.Type == "login_hint")?.Value ?? "";
        if (!Enum.TryParse(authState.User.FindFirst(c => c.Type == "IdentityProvider")?.Value, out IdentityProvider identityProvider))
            identityProvider = IdentityProvider.Azure;

        var redirectLogoutUrl = $"{Navigation.BaseUri}landing_page";
        await AuditClient.PostLogAsync(new PostLogRequest { Message = $"Logout logoutHint: `{logoutHint}` redirectLogoutUrl: `{redirectLogoutUrl}`" });
        string urlLogout;
        switch (identityProvider)
        {
            case IdentityProvider.Azure:
                if (string.IsNullOrEmpty(logoutHint))
                    urlLogout = $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={redirectLogoutUrl}";
                else
                    urlLogout = $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?logout_hint={logoutHint}&post_logout_redirect_uri={redirectLogoutUrl}";
                break;
            case IdentityProvider.KeyCloak:
                // ToDo: configurable logout url for KeyCloak
                var idToken = authState.User?.FindFirst(c => c.Type == "idToken")?.Value ?? "";
                if (string.IsNullOrEmpty(idToken))
                    urlLogout = $"https://keycloaktest.droogers.cloud/realms/master/protocol/openid-connect/logout?post_logout_redirect_uri={redirectLogoutUrl}";
                else
                    urlLogout = $"https://keycloaktest.droogers.cloud/realms/master/protocol/openid-connect/logout?id_token_hint={idToken}&post_logout_redirect_uri={redirectLogoutUrl}";
                break;
            case IdentityProvider.None:
            default:
                await AuditClient.PostLogAsync(new PostLogRequest { Message = $"Unknown identity provider: '{identityProvider}' while logging out" });
                return;
        }

        await AuthenticationStateProvider.Logout();
        StateHasChanged();
        Navigation.NavigateTo(urlLogout);
    }

    private async Task LoginAzure(GetLoginSecretsResponse secrets, string redirectUrl)
    {
        var tenant = secrets.TenantId;
        var clientId = secrets.ClientId;
        var responseType = "code";
        var responseMode = "query";
        var scope = "openid+profile+email";
        var code_challenge_method = "S256";
        var url =
            $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?client_id={clientId}&response_type={responseType}&redirect_uri={redirectUrl}&response_mode={responseMode}&scope={scope}&state={secrets.LoginSecret}&nonce={secrets.LoginNonce}&code_challenge={secrets.CodeChallenge}&code_challenge_method={code_challenge_method}";
        Navigation.NavigateTo(url);
    }

    private async Task LoginKeyCloak(GetLoginSecretsResponse secrets, string redirectUrl)
    {
        var instance = secrets.Instance;
        var tenant = secrets.TenantId;
        var clientId = secrets.ClientId;
        var responseType = "code";
        var responseMode = "query";
        var scope = "openid+profile+email";
        var code_challenge_method = "S256";
        var url =
            $"{instance}/realms/{tenant}/protocol/openid-connect/auth?client_id={clientId}&response_type={responseType}&redirect_uri={redirectUrl}&response_mode={responseMode}&scope={scope}&state={secrets.LoginSecret}&nonce={secrets.LoginNonce}&code_challenge={secrets.CodeChallenge}&code_challenge_method={code_challenge_method}";
        Navigation.NavigateTo(url);
    }
}