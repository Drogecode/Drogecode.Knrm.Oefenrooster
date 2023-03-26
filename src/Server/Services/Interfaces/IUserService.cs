using Drogecode.Knrm.Oefenrooster.Shared.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService
{
    Task<List<DrogeUser>> GetAllUsers(Guid customerId);
    Task<DrogeUser?> GetUserFromDb(Guid userId);
    Task<DrogeUser> GetOrSetUserFromDb(Guid userId, string userName, string userEmail, Guid customerId);
    Task<bool> AddUser(DrogeUser user, Guid customerId);
    Task<bool> UpdateUser(DrogeUser user, Guid userId, string userName, string userEmail, Guid customerId);
    Task<bool> MarkUsersDeleted(List<DrogeUser> existingUsers, Guid userId, Guid customerId);
}
