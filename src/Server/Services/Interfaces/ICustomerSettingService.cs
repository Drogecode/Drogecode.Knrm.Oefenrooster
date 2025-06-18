using Drogecode.Knrm.Oefenrooster.Server.Models.Authentication;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface ICustomerSettingService
{
    Task<SettingBoolResponse> GetBoolCustomerSetting(Guid customerId, SettingName setting, bool def, CancellationToken clt);
    Task<SettingIntResponse> GetIntCustomerSetting(Guid customerId, SettingName setting, int def, CancellationToken clt);
    Task<string> GetTimeZone(Guid customerId);
    Task<SettingStringResponse> GetStringCustomerSetting(Guid customerId, SettingName setting, string def, CancellationToken clt);
    Task<SettingListIntResponse> GetListIntCustomerSetting(Guid customerId, SettingName setting, List<int> def);
    Task PatchBoolSetting(Guid customerId, SettingName name, bool value);
    Task PatchIntSetting(Guid customerId, SettingName setting, int value);
    Task PatchStringSetting(Guid customerId, SettingName setting, string value);
}
