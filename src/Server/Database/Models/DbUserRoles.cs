﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserRoles")]
public class DbUserRoles
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid? ExternalId { get; set; }
    [StringLength(50)] public string? Name { get; set; }
    [StringLength(500)] public string? Accesses { get; set; }
    public int Order { get; set; }

    public DbCustomers Customer { get; set; }
    public ICollection<DbLinkUserRole>? LinkUserRoles { get; set; }
}
