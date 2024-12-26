using System.IdentityModel.Tokens.Jwt;

namespace Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;

public class AuthenticateUserResult
{
    internal bool Success { get; set; }
    internal string IdToken { get; set; } = string.Empty;
    internal string RefreshToken { get; set; } = string.Empty;
    internal JwtSecurityToken? JwtSecurityToken { get; set; }
}