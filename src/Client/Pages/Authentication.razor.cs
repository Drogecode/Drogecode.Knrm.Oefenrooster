using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;

public sealed partial class Authentication
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private CustomStateProvider CustomStateProvider { get; set; } = default!;
    [Inject] private IAuthenticationClient AuthenticationClient { get; set; } = default!;
    [Parameter] public string? Action { get; set; }
    [Parameter] [SupplyParameterFromQuery] public string? code { get; set; }
    [Parameter] [SupplyParameterFromQuery] public string? state { get; set; }
    [Parameter] [SupplyParameterFromQuery] public string? session_state { get; set; }

    //https://learn.microsoft.com/nl-nl/azure/active-directory/develop/v2-protocols-oidc
    //https://codewithmukesh.com/blog/authentication-in-blazor-webassembly/
    protected override async Task OnParametersSetAsync()
    {
        var redirectUrl = $"{Navigation.BaseUri}authentication/login-callback";
        switch (Action)
        {
            case "login":
                var secrets = await AuthenticationClient.GetLoginSecretsAsync();
                var tenant = "d9754755-b054-4a9c-a77f-da42a4009365";
                var clientId = "1ca47bae-d6c1-495e-9b90-c32b244fdde2";
                var responseType = "code";
                var responseMode = "query";
                var scope = "openid+profile+email";
                var code_challenge_method = "S256";
                var url = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?client_id={clientId}&response_type={responseType}&redirect_uri={redirectUrl}&response_mode={responseMode}&scope={scope}&state={secrets.LoginSecret}&nonce={secrets.LoginNonce}&code_challenge={secrets.CodeChallenge}&code_challenge_method={code_challenge_method}";
                Navigation.NavigateTo(url);
                break;
            case "login-callback":

                //https://localhost:44327/authentication/login-callback?
                //code=0.ATAAVUd12VSwnEqnf9pCpACTZa57pBzB1l5Jm5DDKyRP3eIwAFY.AgABAAIAAAAtyolDObpQQ5VtlI4uGjEPAgDs_wUA9P_0knpzIDaOclMFbZVWvzgkrOGgoKDWAkpTpALiDDTLmgIywpb0VXuMvskBVO3cfWNqd9ROR21fGh706ObeExSAitcGOiy3baOutXSpHfXKTfQ2rGtWuKSxTwdq7YacQ7vs9VKvMjCd1nCXkmG8v_5rJfBDHGlFgGJOrwQ79gnOmOjMT3OLFpoLBswNglns6Y09wmVE_bkEmoB2e7-vbDavshWK0KyUkz6uglcolkCVATAefabAEyt88nOuJRqi9F8tPBmeIznnu3FHKtMMolNoTH4il15oaav7VNvc4ea129_qxixCVcrm6SVvCYkRjKnIr0Rf2sog-MgHp034aCgcbhawUTYe9X1jsvUc7I9MQYjY811vSsMECvO0uzjaZapSkRWFhoc3i1KpyNjpOqn6UkvB7UUe-CzXPaxm4VaordKmEUjG73bp8-sEK1x5KOQ-zhF-DsmVOtR0WToItAC93qJryadYFHngEj3tCqv8oFiCiv2_tWOgkoFeECaKGeFIQx0FUOtcaWfz9Fub4CJfVH6yQOoL-QcyjGnbWfvYx2fRZE8gfy4mmpSBcZafbcf-R3IXMvvnhz1NZksLeZpYrwFPvljVNiVa3n4oU-3gBUZvcccmJxDcquPC_oSdOTnlsX9gfgGQvh8NJLnYa97now27rkvryhhwEbdlGi4NJJrXpAPYVs424RqWgST8B-QmVwohI5tKbfuyiSoM4kOXxY1xZuxano3HbU9mPwcZgh794JfLo1f7SLuLJhRMhH5f-jVsFlwU6vrJb9_GIDbP-kRZm9qkyWvvUgsTx8Tmdkobd5HR9priYAyX6K2fdQF4_yuYZyojCce-90K7CntvSdl4lEq0m8rhgLN7cR4u1ZtHidtyxci0p41Y8smhKMbLPLbyIsBGx7pe55Q
                //&state=WIAge8uGpFQ5RTNLZ9gEy9r0gAB6OgFJBiJOZ9Sn9TomA8a6QXABWfcrv51T3HG3
                //&session_state=524653cd-d523-4038-a704-6e58bb555568
                await AuthenticationClient.AuthenticatUserAsync(code, state, session_state, redirectUrl);
                await CustomStateProvider.loginCallback();
                Navigation.NavigateTo("/");
                break;
            case "logout":
                await CustomStateProvider.Logout();
                var redirectLogoutUrl = $"{Navigation.BaseUri}";
                var urlLogout = $"https://login.microsoftonline.com/common/oauth2/v2.0/logout?post_logout_redirect_uri={redirectLogoutUrl}";
                Navigation.NavigateTo(urlLogout);
                break;
        }
    }
}
