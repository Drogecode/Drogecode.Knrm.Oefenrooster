using Drogecode.Knrm.Oefenrooster.Server.Models.User;
using Drogecode.Knrm.Oefenrooster.Server.Services.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService : IDrogeService
{
    Task<MultipleDrogeUsersResponse> GetAllUsers(Guid customerId, bool includeHidden, bool includeLastLogin, CancellationToken clt);
    Task<DrogeUser?> GetUserById(Guid customerId, Guid userId, bool includePersonal, CancellationToken clt);
    Task<DrogeUser?> GetUserByPreComId(int preComUserId, CancellationToken clt);
    Task<DrogeUserServer?> GetUserByNameForServer(string? name, CancellationToken clt);
    Task<DrogeUserServer?> GetOrSetUserById(Guid? userId, string? externalId, string userName, string userEmail, Guid customerId, bool setLastOnline, CancellationToken clt);
    Task<AddUserResponse> AddUser(DrogeUser user, Guid customerId);
    Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId);
    Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt);
    Task<UpdateLinkUserUserForUserResponse> RemoveLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt);
    Task<bool> PatchLastOnline(Guid? userId, Guid? customerId, string? clientVersion, CancellationToken clt);
    Task<bool> PatchLastOnline(Guid userId, CancellationToken clt);
    Task<bool> MarkUserDeleted(DrogeUser user, Guid userId, Guid customerId, bool onlySyncedFromSharePoint);
    Task<bool> MarkMultipleUsersDeleted(List<DrogeUser> existingUsers, Guid userId, Guid customerId, bool onlySyncedFromSharePoint);
}
