using Drogecode.Knrm.Oefenrooster.Server.Models.UserPreCom;
using Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

namespace Drogecode.Knrm.Oefenrooster.Server.Services.Interfaces;

public interface IUserSettingService
{
    Task<SettingBoolResponse> GetBoolUserSetting(Guid customerId, Guid userId, SettingName setting, bool def, CancellationToken clt);
    Task<SettingStringResponse> GetStringUserSetting(Guid customerId, Guid userId, SettingName setting, string def, CancellationToken clt);
    Task PatchBoolSetting(Guid customerId, Guid userId, SettingName setting, bool value);
    Task PatchStringSetting(Guid customerId, Guid userId, SettingName setting, string value);
    Task<List<UserPreComIdAndValue>> GetAllPreComIdAndValue(Guid customerId, SettingName setting, CancellationToken clt);
}
