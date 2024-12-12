using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IAuthenticationService
{
     Task<GetLoginSecretsResponse> GetLoginSecrets();
}