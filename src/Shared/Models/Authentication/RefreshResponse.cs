namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public class RefreshResponse : BaseResponse
{
    public RefreshState State { get; set; }
    /// <summary>
    /// Refreshing creates a reload in the client.
    /// </summary>
    public bool ForceRefresh { get; set; }
}

public enum RefreshState
{
    None = 0,
    CurrentAuthenticationValid = 1,
    CurrentAuthenticationExpired = 2,
    NotAuthenticated = 3,
    NoRefreshToken = 4,
    RefreshFailed = 4,
    AuthenticationRefreshed = 5,
}