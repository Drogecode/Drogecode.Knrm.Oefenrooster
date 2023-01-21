using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Database.Models;

[Table("Users")]
public class DbUsers
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }
}
