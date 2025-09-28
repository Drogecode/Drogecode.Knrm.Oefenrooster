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
    public TrainingTargetGroup Group { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_NAME)] public string? Name { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_DESCRIPTION)] public string? Description { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_URL)] public string? Url { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_URL_Description)] public string? UrlDescription { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    
    public DbCustomers Customer { get; set; }
    public DbTrainingTargetSubjects Subject { get; set; }
    public ICollection<DbTrainingTargetUserResult>? TrainingTargetUserResults { get; set; }
}