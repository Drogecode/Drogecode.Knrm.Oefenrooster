using System.ComponentModel.DataAnnotations;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbLinkUserRole
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public bool IsSet { get; set; }
    public bool SetExternal { get; set; }
    
    public DbUsers User { get; set; } = null!;
    public DbUserRoles Role { get; set; } = null!;
}