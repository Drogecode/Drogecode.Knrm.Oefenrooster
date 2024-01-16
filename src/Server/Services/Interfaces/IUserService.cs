using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService
{
    Task<MultipleDrogeUsersResponse> GetAllUsers(Guid customerId, bool includeHidden, bool includeLastLogin);
    Task<DrogeUser?> GetUserFromDb(Guid userId);
    Task<DrogeUser> GetOrSetUserFromDb(Guid userId, string userName, string userEmail, Guid customerId, bool setLastOnline);
    Task<AddUserResponse> AddUser(DrogeUser user, Guid customerId);
    Task<bool> UpdateUser(DrogeUser user, Guid userId, Guid customerId);
    Task<UpdateLinkUserUserForUserResponse> UpdateLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId);
    Task<UpdateLinkUserUserForUserResponse> RemoveLinkUserUserForUser(UpdateLinkUserUserForUserRequest body, Guid userId, Guid customerId);
    Task<bool> PatchLastOnline(Guid userId, CancellationToken clt);
    Task<bool> MarkUsersDeleted(List<DrogeUser> existingUsers, Guid userId, Guid customerId);
}
