using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("LinkReportTrainingRoosterTraining")]
public class DbLinkReportTrainingRoosterTraining
{
    [Key] public required Guid Id { get; set; }
    public required Guid RoosterTrainingId { get; set; }
    public required Guid ReportTrainingId { get; set; }
    public required Guid CustomerId { get; set; }
    /// <summary>
    /// Indicates if or when the report was synced from the external system.
    /// </summary>
    public DateTime? SetByExternalOn { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    
    public DbRoosterTraining RoosterTraining { get; set; }
    public DbReportTraining ReportTraining { get; set; }
    public DbCustomers Customer { get; set; }
}