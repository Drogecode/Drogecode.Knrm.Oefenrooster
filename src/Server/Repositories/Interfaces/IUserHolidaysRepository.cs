using Drogecode.Knrm.Oefenrooster.Server.Database.Models;

namespace Drogecode.Knrm.Oefenrooster.Server.Repositories.Interfaces;

public interface IUserHolidaysRepository
{
    Task<List<DbUserHolidays>> GetUserHolidaysForUser(bool cache, Guid customerId, Guid? userId, DateTime tillDate, DateTime startDate, CancellationToken clt);
}