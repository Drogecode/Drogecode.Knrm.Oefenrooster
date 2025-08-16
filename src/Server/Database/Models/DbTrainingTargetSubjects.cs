using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("TrainingTargetSubjects")]
public class DbTrainingTargetSubjects
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? ParentId { get; set; }
    public int Order { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_TRAINING_TARGET_SUBJECT_NAME)] public string? Name { get; set; }
    
    public DbCustomers Customer { get; set; }
    public DbTrainingTargetSubjects? Parent { get; set; }
    public ICollection<DbTrainingTargets>? TrainingTargets { get; set; }
    public ICollection<DbTrainingTargetSubjects>? Children { get; set; }
}