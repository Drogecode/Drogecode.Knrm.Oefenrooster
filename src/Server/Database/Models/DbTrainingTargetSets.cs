using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("TrainingTargetSets")]
public class DbTrainingTargetSets
{
    [Key] public Guid Id { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_SET_NAME)]public string? Name { get; set; }
    public List<Guid> TrainingTargetIds { get; set; } = [];
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public Guid CreatedBy { get; set; }
    
    public ICollection<DbRoosterTraining>? RoosterTrainings { get; set; }
}