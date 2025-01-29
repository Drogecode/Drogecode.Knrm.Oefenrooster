using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerSettingService
{
    Task<Customer> GetByTenantId(string externalCustomerId, CancellationToken clt);
    Task<SettingBoolResponse> GetBoolCustomerSetting(Guid customerId, SettingName name);
    Task<string> GetTimeZone(Guid customerId);
    Task<SettingStringResponse> GetStringCustomerSetting(Guid customerId, SettingName name, string def);
    Task PatchBoolSetting(Guid customerId, SettingName name, bool value);
    Task PatchStringSetting(Guid customerId, SettingName setting, string value);
}
