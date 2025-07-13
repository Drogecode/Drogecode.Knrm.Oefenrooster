using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract.Interfaces;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;

public interface IAuthenticationManager : IDrogeManager
{
    IAuthenticationService GetAuthenticationService();
}