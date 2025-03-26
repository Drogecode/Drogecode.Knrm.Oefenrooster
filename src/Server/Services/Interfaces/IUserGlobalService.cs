using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserGlobalService
{
    Task<AllDrogeUserGlobalResponse> GetAllUserGlobals(CancellationToken clt);
}