using Drogecode.Knrm.Oefenrooster.Shared.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;
public interface IUserService
{
    User? GetUserFromDb(Guid userId, string userName);
    User GetOrSetUserFromDb(Guid userId, string userName);
}
