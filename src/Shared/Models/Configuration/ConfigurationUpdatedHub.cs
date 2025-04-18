using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.Configuration;

public class ConfigurationUpdatedHub
{
    public Guid? ByUserId { get; set; }
    public ConfigurationUpdated ConfigurationUpdated  { get; set; }
}