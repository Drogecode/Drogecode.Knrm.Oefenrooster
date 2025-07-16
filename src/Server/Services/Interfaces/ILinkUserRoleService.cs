using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ILinkUserRoleService
{
    Task<List<Guid>> GetLinkUserRolesAsync(Guid userId, bool onlyExternal, CancellationToken clt);
    Task<GetLinkedUsersByIdResponse> GetLinkedUsersById(Guid roleId, Guid customerId, CancellationToken clt);
    Task<bool> LinkUserToRoleAsync(Guid userId, DrogeUserRoleBasic? role, bool isSet, bool setExternal, bool modifySetByExternal, CancellationToken clt);
}