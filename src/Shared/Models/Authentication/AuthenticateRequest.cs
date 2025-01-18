namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public class AuthenticateRequest(string? code, string? state, string? sessionState, string redirectUrl, string clientVersion)
{
    public string? Code { get; set; } = code;
    public string? State { get; set; } = state;
    public string? SessionState { get; set; } = sessionState;
    public string RedirectUrl { get; set; } = redirectUrl;
    public string ClientVersion { get; set; } = clientVersion;
}