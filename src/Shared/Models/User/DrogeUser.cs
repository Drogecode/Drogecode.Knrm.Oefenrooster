using Drogecode.Knrm.Oefenrooster.Shared.Enums;

namespace Drogecode.Knrm.Oefenrooster.Shared.Models.User;

public class DrogeUser
{
    public Guid Id { get; set; }
    public string? ExternalId { get; set; }
    public Guid CustomerId { get; set; }
    public string? Buddy { get; set; }
    public string Name { get; set; }
    public List<string>? Versions { get; set; }
    public int? Nr { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastLogin { get; set; }
    public Guid? UserFunctionId { get; set; }
    public List<LinkedDrogeUser>? LinkedAsA { get; set; }
    public List<LinkedDrogeUser>? LinkedAsB { get; set; }
    public bool SyncedFromSharePoint { get; set; }
    public bool RoleFromSharePoint { get; set; }
}

public class LinkedDrogeUser
{
    public Guid LinkedUserId { get; set; }
    public UserUserLinkType LinkType { get; set; }
}