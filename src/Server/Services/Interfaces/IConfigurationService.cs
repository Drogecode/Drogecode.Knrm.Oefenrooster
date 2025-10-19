using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Nager.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IConfigurationService
{
    Task<bool> UpgradeDatabase();
    Task<bool> AddSpecialDay(Guid customerId, PublicHoliday holiday, CancellationToken clt);
    Task<DbCorrectionResponse> DbCorrection(CancellationToken clt);
}
