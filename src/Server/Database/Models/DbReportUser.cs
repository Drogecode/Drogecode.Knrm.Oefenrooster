using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbReportUser
{
    [Key] public Guid Id { get; set; }
    public Guid? DbReportActionId { get; set; }
    public Guid? DbReportTrainingId { get; set; }
    [StringLength(10)] public string? SharePointID { get; set; }
    public Guid? DrogeCodeId { get; set; }
    [StringLength(50)] public string? Name { get; set; }
    public SharePointRole Role { get; set; }
    public bool IsDeleted { get; set; }
    public int Order { get; set; }
    
    public DbReportAction? Action { get; set; }
    public DbReportTraining? Training { get; set; }
    
    [NotMapped] public bool IsNew { get; set; }
}
