using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserLogins")]
public class DbUserLogins
{
    [Key] public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? SharedActionId { get; set; }
    public DateTime LoginDate { get; set; }
    [StringLength(50)] public string? Ip { get; set; }
    public bool? DirectLogin { get; set; }
    [StringLength(20)] public string? Version { get; set; }
    
    public DbUsers? User { get; set; }
    public DbReportActionShared? SharedAction { get; set; }
}