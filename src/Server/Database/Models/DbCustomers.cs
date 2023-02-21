using Drogecode.Knrm.Oefenrooster.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Customers")]
public class DbCustomers
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime Created { get; set; }

    public ICollection<DbUsers>? Users { get; set; }
    public ICollection<DbUserFunctions>? UserFunctions { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbRoosterDefault>? RoosterDefaults { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTraining { get; set; }
    public ICollection<DbAudit>? Audits { get; set; }
    public ICollection<DbVehicles>? Vehicles { get; set; }
    public ICollection<DbLinkVehicleTraining>? LinkVehicleTraining{ get; set; }
}
