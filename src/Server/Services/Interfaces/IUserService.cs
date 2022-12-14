using Drogecode.Knrm.Oefenrooster.Shared.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService
{
    DrogeUser? GetUserFromDb(Guid userId);
    DrogeUser GetOrSetUserFromDb(Guid userId, string userName, string userEmail);
}
