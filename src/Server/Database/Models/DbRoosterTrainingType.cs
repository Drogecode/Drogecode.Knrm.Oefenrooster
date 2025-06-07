using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterTrainingType")]
public class DbRoosterTrainingType
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_DAY_ITEM_TEXT)] public string? Name { get; set; }
    [StringLength(30)] public string? ColorLight { get; set; }
    [StringLength(30)] public string? ColorDark { get; set; }
    [StringLength(30)] public string? TextColorLight { get; set; }
    [StringLength(30)] public string? TextColorDark { get; set; }
    public int Order { get; set; }
    public bool CountToTrainingTarget { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
    public ICollection<DbRoosterDefault>? RoosterDefaults { get; set; }
}