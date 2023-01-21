using Drogecode.Knrm.Oefenrooster.Shared.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService
{
    Task<DrogeUser?> GetUserFromDb(Guid userId);
    Task<DrogeUser> GetOrSetUserFromDb(Guid userId, string userName, string userEmail);
}
