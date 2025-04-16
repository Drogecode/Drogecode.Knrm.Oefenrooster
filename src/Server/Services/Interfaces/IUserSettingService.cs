using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserSettingService
{
    Task<SettingBoolResponse> GetBoolUserSetting(Guid customerId, Guid userId, SettingName setting);
    Task<SettingStringResponse> GetStringUserSetting(Guid customerId, Guid userId, SettingName setting);
    Task PatchBoolSetting(Guid customerId, Guid userId, SettingName setting, bool value);
    Task PatchStringSetting(Guid customerId, Guid userId, SettingName setting, string value);
    Task<List<SyncPreComWithCalendarSetting>> GetAllSyncPreComWithCalendarSetting(Guid customerId, CancellationToken clt);
}
