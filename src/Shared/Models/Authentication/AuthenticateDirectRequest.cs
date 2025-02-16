namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public struct AuthenticateDirectRequest
{
    public string? Name { get; set; }
    public string? Password { get; set; }
    
    public string ClientVersion { get; set; }
}