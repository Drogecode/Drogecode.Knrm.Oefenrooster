using Drogecode.Knrm.Oefenrooster.Client.Services;
using Drogecode.Knrm.Oefenrooster.ClientGenerator.Client;

namespace Drogecode.Knrm.Oefenrooster.Client.Pages;

public sealed partial class Authentication
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private CustomStateProvider CustomStateProvider { get; set; } = default!;
    [Inject] private IAuthenticationClient AuthenticationClient { get; set; } = default!;
    [Parameter] public string? Action { get; set; }

    //https://learn.microsoft.com/nl-nl/azure/active-directory/develop/v2-protocols-oidc
    //https://codewithmukesh.com/blog/authentication-in-blazor-webassembly/
    protected override async Task OnParametersSetAsync()
    {
        switch (Action)
        {
            case "login":
                var secrets = await AuthenticationClient.GetLoginSecretsAsync();
                var tenant = "d9754755-b054-4a9c-a77f-da42a4009365";
                var clientId = "1ca47bae-d6c1-495e-9b90-c32b244fdde2";
                var responseType = "id_token";
                var redirectUrl = $"{Navigation.BaseUri}api/authentication/login-callback";
                var responseMode = "form_post";
                var scope = "openid+profile+email";
                var url = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/authorize?client_id={clientId}&response_type={responseType}&redirect_uri={redirectUrl}&response_mode={responseMode}&scope={scope}&state={secrets.LoginSecret}&nonce={secrets.LoginNonce}";
                Navigation.NavigateTo(url);
                break;
            case "login-callback":
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
