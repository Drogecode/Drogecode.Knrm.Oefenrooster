﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("CustomerSettings")]
public class DbCustomerSettings
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public SettingName Name { get; set; } = SettingName.None;
    [StringLength(50)] public string? Value { get; set; }

    public DbCustomers Customer { get; set; }
}