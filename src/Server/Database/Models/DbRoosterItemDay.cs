﻿using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Models.CalendarItem;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterItemDay")]
public class DbRoosterItemDay : RoosterItemDay
{
    public Guid CustomerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers? User { get; set; }
}
