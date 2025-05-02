using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

[Serializable]
public class PatchSettingIntRequest(SettingName name, int value)
{
    public SettingName Name { get; set; } = name;
    public int Value { get; set; } = value;
}