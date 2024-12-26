namespace Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;

public class DrogeClaims
{
    internal string Email { get; set; }
    internal string FullName { get; set; }
    internal string ExternalUserId { get; set; }
    internal string LoginHint { get; set; }
    internal string ExternalCustomerId { get; set; }
}