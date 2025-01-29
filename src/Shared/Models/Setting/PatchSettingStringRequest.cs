using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Setting;

public class PatchSettingStringRequest(SettingName name, string value)
{
    public SettingName Name { get; set; } = name;
    public string Value { get; set; } = value;
}