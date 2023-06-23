using Drogecode.Knrm.Oefenrooster.Shared.Models.Auth;

namespace Drogecode.Knrm.Oefenrooster.Client.Services.Interfaces;

public interface IAuthService
{
    Task Login(LoginRequest loginRequest);
    Task Register(RegisterRequest registerRequest);
    Task Logout();
    Task<CurrentUser> CurrentUserInfo();
}
