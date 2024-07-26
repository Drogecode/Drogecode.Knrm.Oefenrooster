using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IHolidayService
{
    Task<MultipleHolidaysResponse> GetAllHolidaysForUser(Guid customerId, Guid userId, CancellationToken clt);
    Task<MultipleHolidaysResponse> GetAllHolidaysForFuture(Guid customerId, Guid userId, int days, CancellationToken clt);
    Task<GetResponse> Get(Guid id, Guid customerId, Guid userId, CancellationToken clt);
    Task<PutHolidaysForUserResponse> PutHolidaysForUser(Holiday body, Guid customerId, Guid userId, CancellationToken clt);
    Task<PatchHolidaysForUserResponse> PatchHolidaysForUser(Holiday body, Guid customerId, Guid userId, CancellationToken clt);
    Task<DeleteResonse> Delete(Guid id, Guid customerId, Guid userId, CancellationToken clt);
}
