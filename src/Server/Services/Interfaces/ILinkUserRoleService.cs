using Drogecode.Knrm.Oefenrooster.Shared.Models.UserRole;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ILinkUserRoleService
{
    Task<List<Guid>> GetLinkUserRolesAsync(Guid userId, CancellationToken clt);
    Task<GetLinkedUsersByIdResponse> GetLinkedUsersById(Guid roleId, Guid customerId, CancellationToken clt);
    Task<bool> LinkUserToRoleAsync(Guid userId, Guid roleId, bool isSet, bool setExternal, CancellationToken clt);
}