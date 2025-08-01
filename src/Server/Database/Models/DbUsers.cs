﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;

namespace Drogecode.Knrm.Oefenrooster.Server.Database.Models;

[Table("Users")]
public class DbUsers
{
    [Key] public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    [StringLength(50)] public string? ExternalId { get; set; }
    public int? PreComId { get; set; }
    public Guid? UserFunctionId { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_USER_NAME)] public string Name { get; set; }
    [StringLength(DefaultSettingsHelper.MAX_LENGTH_USER_EMAIL)] public string Email { get; set; }
    public int? Nr { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    [StringLength(10)] public string? SharePointID { get; set; }
    public bool SyncedFromSharePoint { get; set; }
    public bool RoleFromSharePoint { get; set; }
    public bool IsSystemUser { get; set; }
    public bool DirectLogin { get; set; }
    [StringLength(200)] public string? HashedPassword { get; set; }

    public DbCustomers Customer { get; set; }
    public DbUserFunctions? UserFunction { get; set; }
    public ICollection<DbRoosterAvailable>? RoosterAvailables { get; set; }
    public ICollection<DbRoosterItemDay>? RoosterItemDays { get; set; }
    public ICollection<DbLinkUserDayItem>? LinkUserDayItems { get; set; }
    public ICollection<DbUsers>? LinkedUserA { get; set; }
    public ICollection<DbUsers>? LinkedUserB { get; set; }
    public ICollection<DbLinkUserUser>? LinkedUserAsA { get; set; }
    public ICollection<DbLinkUserUser>? LinkedUserAsB { get; set; }
    public ICollection<DbPreComForward>? PreComForwards { get; set; }
    public ICollection<DbPreComAlert>? PreComAlerts { get; set; }
    public ICollection<DbUserDefaultGroup>? UserDefaultGroups { get; set; }
    public ICollection<DbUserDefaultAvailable>? UserDefaultAvailables { get; set; }
    public ICollection<DbUserHolidays>? UserHolidays { get; set; }
    public ICollection<DbUserOnVersion>? UserOnVersions { get; set; }
    public ICollection<DbUserSettings>? UserSettings { get; set; }
    public ICollection<DbUserLinkedMails>? UserLinkedMails { get; set; }
    public ICollection<DbAudit>? Audits { get; set; }
    public ICollection<DbLinkUserRole>? LinkUserRoles { get; set; }
    public ICollection<DbUserLogins>? Logins { get; set; }
    public ICollection<DbRoosterTraining>? TrainingsDeleted { get; set; }
    public ICollection<DbRoosterAvailable>? TrainingAvailableLastUpdated { get; set; }
    public ICollection<DbUserLastCalendarUpdate>? UserLastCalendarUpdates { get; set; }
    public ICollection<DbLinkUserCustomer>? LinkUserCustomers { get; set; }
    public ICollection<DbUserPreComEvent>? UserPreComEvent { get; set; }
}