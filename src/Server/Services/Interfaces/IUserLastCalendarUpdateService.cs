using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserLastCalendarUpdateService
{
    Task<bool> AddOrUpdateLastUpdateUser(Guid customerId, Guid userId, CancellationToken clt);
    Task<List<DbUserLastCalendarUpdate>> GetLastUpdateUsers(CancellationToken clt);
}