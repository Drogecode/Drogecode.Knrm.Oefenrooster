using System.IdentityModel.Tokens.Jwt;
using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IAuthenticationService
{
     Task<GetLoginSecretsResponse> GetLoginSecrets();
     Task<AuthenticateUserResult> AuthenticateUser(CacheLoginSecrets found, string code, string state, string sessionState, string redirectUrl, CancellationToken clt);
     Task<AuthenticateUserResult> Refresh(string oldRefreshToken, CancellationToken clt);
     DrogeClaims GetClaims(JwtSecurityToken jwtSecurityToken);
}