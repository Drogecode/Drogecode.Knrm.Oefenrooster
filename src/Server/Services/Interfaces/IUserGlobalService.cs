using Drogecode.Knrm.Oefenrooster.Shared.Models.UserGlobal;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserGlobalService
{
    Task<AllDrogeUserGlobalResponse> GetAllUserGlobals(CancellationToken clt);
    Task<GetGlobalUserByIdResponse> GetGlobalUserById(Guid id, CancellationToken clt);
    Task<PutResponse> PutGlobalUser(Guid userId, DrogeUserGlobal globalUser, CancellationToken clt);
    Task<PatchResponse> PatchGlobalUser(Guid userId, DrogeUserGlobal globalUser, CancellationToken clt);
}