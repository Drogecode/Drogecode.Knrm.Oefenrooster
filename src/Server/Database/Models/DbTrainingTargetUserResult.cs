using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("TrainingTargetUserResult")]
public class DbTrainingTargetUserResult
{
    [Key] public Guid Id { get; set; }
    public Guid TrainingTargetId { get; set; }
    public Guid RoosterAvailableId { get; set; }
    public Guid UserId { get; set; }
    public int Result { get; set; }
    public bool SetInBulk { get; set; }
    public DateTime? TrainingDate { get; set; }
    public DateTime? ResultDate { get; set; }
    public Guid SetBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    
    public DbTrainingTargets TrainingTarget { get; set; }
    public DbRoosterAvailable RoosterAvailable { get; set; }
    public DbUsers User { get; set; }
}