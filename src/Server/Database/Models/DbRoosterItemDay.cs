using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterItemDay")]
public class DbRoosterItemDay
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime Date { get; set; }
    public bool IsFullDay { get; set; }
    public CalendarItemType Type { get; set; }
    public string Text { get; set; } = string.Empty;

    public DbCustomers Customer { get; set; }
}
