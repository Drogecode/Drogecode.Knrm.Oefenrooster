using Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;
using Nager.Holiday;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IConfigurationService
{
    Task<bool> UpgradeDatabase();
    Task<bool> AddSpecialDay(Guid customerId, PublicHoliday holiday, CancellationToken token);
    Task<DbCorrectionResponse> DbCorrection1(CancellationToken clt);
}
