using System.ComponentModel.DataAnnotations;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

public class DbReportUser
{
    [Key] public Guid Id { get; set; }
    public string? SharePointID { get; set; }
    public Guid DrogeCodeId { get; set; }
    public string? Name { get; set; }
    public SharePointRole Role { get; set; }
    public bool IsDeleted { get; set; }
}
