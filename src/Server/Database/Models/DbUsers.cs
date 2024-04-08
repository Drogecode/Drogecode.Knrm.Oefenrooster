using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Users")]
public class DbUsers
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? UserFunctionId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public int? Nr {  get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public string? SharePointID { get; set; }
    public bool SyncedFromSharePoint { get; set; }
    public bool RoleFromSharePoint { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUserFunctions? UserFunction{ get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbRoosterItemDay>? RoosterItemDays { get; set; }
    public ICollection<DbLinkUserDayItem>? LinkUserDayItems { get; set; }
    public ICollection<DbUsers>? LinkedUserA { get; set; }
    public ICollection<DbUsers>? LinkedUserB { get; set; }
    public ICollection<DbLinkUserUser>? LinkedUserAsA { get; set; }
    public ICollection<DbLinkUserUser>? LinkedUserAsB { get; set; }
    public ICollection<DbUserDefaultGroup>? UserDefaultGroups { get; set; }
    public ICollection<DbUserDefaultAvailable>? UserDefaultAvailables { get; set; }
    public ICollection<DbUserHolidays>? UserHolidays { get; set; }
    public ICollection<DbUserOnVersion>? UserOnVersions { get; set; }
    public ICollection<DbUserSettings>? UserSettings { get; set; }
    public ICollection<DbAudit>? Audits { get; set; }
}
