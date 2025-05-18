using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;

public interface IRoosterDefaultsRepository
{
    Task<List<DbRoosterDefault>> GetDefaultsForCustomerInSpan(bool cache, Guid customerId, DateTime tillDate, DateTime startDate, CancellationToken clt);
}