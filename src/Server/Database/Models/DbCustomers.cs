using Drogecode.Knrm.Oefenrooster.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Customers")]
public class DbCustomers
{
    [Key] public Guid Id { get; set; }
    public string Name { get; set; } = "New customer";
    public string TimeZone { get; set; } = "Europe/Amsterdam";
    public DateTime Created { get; set; }
    public string? Instance {  get; set; }
    public string? Domain {  get; set; }
    public string? TenantId {  get; set; }
    public string? ClientIdServer {  get; set; }
    public string? ClientIdLogin {  get; set; }
    public string? ClientSecretServer {  get; set; }
    public string? ClientSecretLogin {  get; set; }


    public ICollection<DbCustomerSettings>? CustomerSettings { get; set; }
    public ICollection<DbUsers>? Users { get; set; }
    public ICollection<DbReportAction>? ReportActions { get; set; }
    public ICollection<DbReportTraining>? ReportTrainings { get; set; }
    public ICollection<DbUserFunctions>? UserFunctions { get; set; }
    public ICollection<DbUserRoles>? UserRoles { get; set; }
    public ICollection<DbUserDefaultGroup>? UserDefaultGroups { get; set; }
    public ICollection<DbUserDefaultAvailable>? UserDefaultAvailables { get; set; }
    public ICollection<DbUserHolidays>? UserHolidays { get; set; }
    public ICollection<DbUserSettings>? UserSettings { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbRoosterDefault>? RoosterDefaults { get; set; }
    public ICollection<DbRoosterItemDay>? RoosterItemDays { get; set; }
    public ICollection<DbRoosterItemMonth>? RoosterItemMonths { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
    public ICollection<DbRoosterTrainingType>? RoosterTrainingTypes { get; set; }
    public ICollection<DbAudit>? Audits { get; set; }
    public ICollection<DbVehicles>? Vehicles { get; set; }
    public ICollection<DbLinkVehicleTraining>? LinkVehicleTrainings { get; set; }
    public ICollection<DbPreComAlert>? PreComAlerts { get; set; }
    public ICollection<DbLinkExchange>? LinkExchanges { get; set; }
}
