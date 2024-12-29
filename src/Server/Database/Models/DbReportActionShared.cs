using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("ReportActionShared")]
public class DbReportActionShared
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public List<Guid>? SelectedUsers { get; set; }
    public List<string>? Types { get; set; }
    public List<string>? Search { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? ValidUntil { get; set; }
    [StringLength(50)] public string Password { get; set; } = string.Empty;
    
    public DbCustomers Customer { get; set; }
}