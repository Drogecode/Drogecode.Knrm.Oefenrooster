namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public struct AuthenticateExternalRequest(Guid? externalId, string? password, string clientVersion)
{
    public Guid? ExternalId { get; set; } = externalId;
    public string? Password { get; set; } = password;
    public string ClientVersion { get; set; } = clientVersion;
}