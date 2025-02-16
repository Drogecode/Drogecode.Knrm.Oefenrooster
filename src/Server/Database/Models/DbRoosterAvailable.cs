using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterAvailable")]
public class DbRoosterAvailable
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid TrainingId { get; set; }
    public Guid? UserFunctionId { get; set; }
    public Guid? VehicleId { get; set; }
    public Guid? LinkExchangeId { get; set; }
    [StringLength(200)] public string? CalendarEventId { get; set; }
    public DateTime Date { get; set; }
    public Availability? Available { get; set; }
    public AvailabilitySetBy SetBy { get; set; }
    public bool Assigned { get; set; }
    public Guid? LastUpdateBy { get; set; }
    public DateTime? LastUpdateOn { get; set; }
    public DateTime? LastSyncOn { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers? User { get; set; }
    public DbUserFunctions? UserFunction { get; set; }
    public DbRoosterTraining Training { get; set; }
    public DbVehicles? Vehicle { get; set; }
    public DbLinkExchange? LinkExchange { get; set; }
    public DbUsers? LastUpdateByUser { get; set; }
}
