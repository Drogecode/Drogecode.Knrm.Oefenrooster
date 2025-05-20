using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;

public interface IUserDefaultAvailableRepository
{
    Task<List<DbUserDefaultAvailable>> GetUserDefaultAvailableForCustomerInSpan(bool cache, Guid customerId, Guid? userId, DateTime tillDate, DateTime startDate, CancellationToken clt);
}