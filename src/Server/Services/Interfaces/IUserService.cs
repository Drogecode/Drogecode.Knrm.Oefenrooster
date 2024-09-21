using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService
{
    Task<MultipleDrogeUsersResponse> GetAllUsers(Guid customerId, bool includeHidden, bool includeLastLogin, CancellationToken clt);
    Task<DrogeUser?> GetUserById(Guid userId, CancellationToken clt);
    Task<DrogeUser?> GetOrSetUserById(Guid userId, string userName, string userEmail, Guid customerId, bool setLastOnline);
    Task<AddUserResponse> AddUser(DrogeUser user, Guid customerId);
    Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId);
    Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt);
    Task<UpdateLinkUserUserForUserResponse> RemoveLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId, CancellationToken clt);
    Task<bool> PatchLastOnline(Guid userId, Guid? customerId, string? clientVersion, CancellationToken clt);
    Task<bool> PatchLastOnline(Guid userId, CancellationToken clt);
    Task<bool> MarkUsersDeleted(List<DrogeUser> existingUsers, Guid userId, Guid customerId);
}
