using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerSettingService
{
    Task<Customer> GetByExternalCustomerId(string externalCustomerId, CancellationToken clt);
    Task<bool> IosDarkLightCheck(Guid customerId);
    Task<bool> TrainingToCalendar(Guid customerId);
    Task<string> GetTimeZone(Guid customerId);
    Task<string> TrainingCalenderPrefix(Guid customerId);
    Task Patch_TrainingToCalendar(Guid customerId, bool value);
}
