using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserHolidays")]
public class DbUserHolidays
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public Availability? Available { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_HOLIDAY_NAME)] public string? Description { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers User { get; set; }
}
