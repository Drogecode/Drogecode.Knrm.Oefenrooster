﻿using Drogecode.Knrm.Oefenrooster.Shared.Models.MonthItem;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterItemMonth")]
public class DbRoosterItemMonth : RoosterItemMonth
{
    public Guid CustomerId { get; set; }

    public DbCustomers Customer { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
}
