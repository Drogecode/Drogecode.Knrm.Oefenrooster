using Drogecode.Knrm.Oefenrooster.Shared.Models.User;
using System.Text.Json.Serialization;

namespace Drogecode.Knrm.Oefenrooster.Server.Models.User;

public class DrogeUserServer : DrogeUser
{
    public bool IsNew { get; set; }
    public bool DirectLogin { get; set; }
    
    // This object should not be returned to the client, but to be sure also ignore HashedPassword in json
    [JsonIgnore] public string? HashedPassword { get; set; }
}