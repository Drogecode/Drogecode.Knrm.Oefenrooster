using Drogecode.Knrm.Oefenrooster.Shared.Models.SharePoint;
using System.ComponentModel.DataAnnotations;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbReportAction
{
    [Key] public Guid Id { get; set; }
    public double Number { get; set; }
    public string? ShortDescription { get; set; }
    public string? Prio { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime Start { get; set; }

    public ICollection<DbReportUser>? Users { get; set; }
}
