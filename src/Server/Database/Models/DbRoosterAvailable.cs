using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterAvailable")]
public class DbRoosterAvailable
{
    [Key]
    public Guid Id { get; set; }
    public Guid IdCustomer { get; set; }
    public Guid IdUser { get; set; }
    public short Available { get; set; }

    public DbRoosterTraining Training { get; set; }
}
