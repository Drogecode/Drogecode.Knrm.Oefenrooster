using Drogecode.Knrm.Oefenrooster.Server.Managers.Abstract.Interfaces;
using Drogecode.Knrm.Oefenrooster.Shared.Models.User;

namespace Drogecode.Knrm.Oefenrooster.Server.Managers.Interfaces;

public interface IUserSyncManager : IDrogeManager
{
    Task<SyncAllUsersResponse> SyncAllUsers(Guid userId, Guid customerId, CancellationToken clt);
}