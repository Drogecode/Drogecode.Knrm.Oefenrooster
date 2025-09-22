using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterTraining")]
public class DbRoosterTraining
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? RoosterDefaultId { get; set; }
    public Guid? RoosterTrainingTypeId { get; set; }
    public Guid? TrainingTargetSetId { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TITLE)] public string? Name { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_DESCRIPTION)] public string? Description { get; set; }
    public DateTime DateStart { get; set; }
    public DateTime DateEnd { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool IsPinned { get; set; }
    public bool IsPermanentPinned { get; set; }
    public bool IsLocked { get; set; }
    public bool? ShowTime { get; set; } = true;

    public DbCustomers Customer { get; set; }
    public DbRoosterDefault? RoosterDefault { get; set; }
    public DbRoosterTrainingType? RoosterTrainingType { get; set; }
    public DbUsers? DeletedByUser { get; set; }
    public DbTrainingTargetSets? TrainingTargetSet { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbLinkVehicleTraining>? LinkVehicleTrainings { get; set; }
    public ICollection<DbLinkReportTrainingRoosterTraining>? LinkReportTrainingRoosterTrainings { get; set; }
}
