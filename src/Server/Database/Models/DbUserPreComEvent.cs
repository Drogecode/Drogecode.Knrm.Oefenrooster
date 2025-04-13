﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("UserPreComEvent")]
public class DbUserPreComEvent
{
    [Key] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid CustomerId { get; set; }
    [StringLength(200)] public string? CalendarEventId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public DateOnly Date { get; set; }
    
    public DbUsers User { get; set; }
    public DbCustomers Customer { get; set; }
}