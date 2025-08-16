using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("TrainingTargets")]
public class DbTrainingTargets
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SubjectId { get; set; }
    public int Order { get; set; }
    public TrainingTargetType Type { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_NAME)]public string? Name { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_DESCRIPTION)]public string? Description { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_URL)]public string? Url { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    
    public DbCustomers Customer { get; set; }
    public DbTrainingTargetSubjects Subject { get; set; }
    public ICollection<DbTrainingTargetUserResult>? TrainingTargetUserResults { get; set; }
}