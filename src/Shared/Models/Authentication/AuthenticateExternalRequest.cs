namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public struct AuthenticateExternalRequest(Guid? externalId, string? passwoord, string clientVersion)
{
    public Guid? ExternalId { get; set; } = externalId;
    public string? Passwoord { get; set; } = passwoord;

    public string ClientVersion { get; set; } = clientVersion;
}