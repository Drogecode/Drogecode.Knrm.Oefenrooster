using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Vehicle")]
public class DbVehicles
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? ExchangeId { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_VEHICLE_NAME)] public string Name { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_VEHICLE_CODE)] public string Code { get; set; }
    public int Order { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? Deletedby { get; set; }
    public DateTime? CreatedOn { get; set; }
    public Guid? Createdby { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbLinkVehicleTraining>? LinkVehicleTrainings { get; set; }
}
