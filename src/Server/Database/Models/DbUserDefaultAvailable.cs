﻿using Drogecode.Knrm.Oefenrooster.Database.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserDefaultAvailable")]
public class DbUserDefaultAvailable
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid RoosterDefaultId { get; set; }
    public Availabilty? Available { get; set; }
    public DateTime? From { get; set; }
    public DateTime? Till { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUsers User { get; set; }
    public DbRoosterDefault RoosterDefault { get; set; }
}