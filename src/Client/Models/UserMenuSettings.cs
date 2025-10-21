namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class UserMenuSettings
{
    public bool ConfigurationExpanded { get; set; }
    public Dictionary<Guid, bool> Expanded { get; } = new();
}
