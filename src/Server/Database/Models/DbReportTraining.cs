using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using System.ComponentModel.DataAnnotations;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbReportTraining
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime Start { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbReportUser>? Users { get; set; }
}
