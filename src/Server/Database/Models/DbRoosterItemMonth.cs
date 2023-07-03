using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using MudBlazor;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterItemMonth")]
public class DbRoosterItemMonth : RoosterItemMonth
{
    public Guid CustomerId { get; set; }

    public DbCustomers Customer { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
}
