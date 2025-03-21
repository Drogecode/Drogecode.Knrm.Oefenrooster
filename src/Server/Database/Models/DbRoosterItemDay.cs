﻿using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("RoosterItemDay")]
public class DbRoosterItemDay
{
    [Key] public Guid Id { get; set; }
    public DateTime? DateStart { get; set; }
    public DateTime? DateEnd { get; set; }
    public bool IsFullDay { get; set; }
    public CalendarItemType Type { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_DAY_ITEM_TEXT)] public string Text { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }

    public DbCustomers Customer { get; set; }
    public List<DbUsers>? Users { get; set; }
    public ICollection<DbLinkUserDayItem>? LinkUserDayItems { get; set; }
}
