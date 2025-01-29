using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

public class PatchSettingBoolRequest(SettingName name, bool value)
{
    public SettingName Name { get; set; } = name;
    public bool Value { get; set; } = value;
}