using Drogecode.Knrm.Oefenrooster.Shared.Models.DefaultSchedule;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IHolidayService
{
    Task<List<Holiday>> GetAllHolidaysForUser(Guid customerId, Guid userId);
    Task<PatchHolidaysForUserResponse> PatchHolidaysForUser(Holiday body, Guid customerId, Guid userId);
}
