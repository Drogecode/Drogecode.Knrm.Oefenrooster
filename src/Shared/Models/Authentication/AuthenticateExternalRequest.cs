namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Authentication;

public class AuthenticateExternalRequest
{
    public Guid? ExternalId { get; set; }
    public string? Passwoord { get; set; }
}