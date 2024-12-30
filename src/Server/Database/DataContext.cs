using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Server.Database
{
    public class DataContext(DbContextOptions<DataContext> context) : DbContext(context), IDataProtectionKeyContext
    {
        // To persist key's
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        public DbSet<DbAudit> Audits { get; set; }
        public DbSet<DbUsers> Users { get; set; }
        public DbSet<DbUserFunctions> UserFunctions { get; set; }
        public DbSet<DbUserRoles> UserRoles { get; set; }
        public DbSet<DbUserDefaultGroup> UserDefaultGroups { get; set; }
        public DbSet<DbUserDefaultAvailable> UserDefaultAvailables { get; set; }
        public DbSet<DbUserLogins> UserLogins { get; set; }
        public DbSet<DbUserHolidays> UserHolidays { get; set; }
        public DbSet<DbUserOnVersion> UserOnVersions { get; set; }
        public DbSet<DbUserSettings> UserSettings { get; set; }
        public DbSet<DbUserLinkedMails> UserLinkedMails { get; set; }
        public DbSet<DbCustomers> Customers { get; set; }
        public DbSet<DbCustomerSettings> CustomerSettings { get; set; }
        public DbSet<DbRoosterDefault> RoosterDefaults { get; set; }
        public DbSet<DbRoosterItemDay> RoosterItemDays { get; set; }
        public DbSet<DbRoosterItemMonth> RoosterItemMonths { get; set; }
        public DbSet<DbRoosterTraining> RoosterTrainings { get; set; }
        public DbSet<DbRoosterTrainingType> RoosterTrainingTypes { get; set; }
        public DbSet<DbRoosterAvailable> RoosterAvailables { get; set; }
        public DbSet<DbVehicles> Vehicles { get; set; }
        public DbSet<DbPreComForward> PreComForwards { get; set; }
        public DbSet<DbPreComAlert> PreComAlerts { get; set; }
        public DbSet<DbMenu> Menus { get; set; }

        public DbSet<DbReportActionShared> ReportActionShares { get; set; }
        public DbSet<DbReportAction> ReportActions { get; set; }
        public DbSet<DbReportTraining> ReportTrainings { get; set; }
        public DbSet<DbReportUser> ReportUsers { get; set; }

        public DbSet<DbLinkVehicleTraining> LinkVehicleTraining { get; set; }
        public DbSet<DbLinkUserDayItem> LinkUserDayItems { get; set; }
        public DbSet<DbLinkUserRole> LinkUserRoles { get; set; }
        public DbSet<DbLinkUserUser> LinkUserUsers { get; set; }
        public DbSet<DbLinkExchange> LinkExchanges { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Audit
            modelBuilder.Entity<DbAudit>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbAudit>().HasOne(p => p.User).WithMany(g => g.Audits).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbAudit>().HasOne(p => p.Customer).WithMany(g => g.Audits).HasForeignKey(s => s.CustomerId).IsRequired();

            // Customers
            modelBuilder.Entity<DbCustomers>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(e => { e.Property(en => en.Name).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(e => { e.Property(en => en.Created).IsRequired(); });

            // CustomerSettings
            modelBuilder.Entity<DbCustomerSettings>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbCustomerSettings>().HasOne(p => p.Customer).WithMany(g => g.CustomerSettings).HasForeignKey(s => s.CustomerId).IsRequired();

            // Users
            modelBuilder.Entity<DbUsers>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUsers>(e => { e.Property(en => en.Name).IsRequired(); });
            modelBuilder.Entity<DbUsers>(e => { e.Property(en => en.Email).IsRequired(); });
            modelBuilder.Entity<DbUsers>().HasOne(p => p.Customer).WithMany(g => g.Users).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUsers>().HasOne(p => p.UserFunction).WithMany(g => g.Users).HasForeignKey(s => s.UserFunctionId);
            modelBuilder.Entity<DbUsers>().HasMany(p => p.LinkedUserA).WithMany(g => g.LinkedUserB)
                .UsingEntity<DbLinkUserUser>(
                    l => l.HasOne<DbUsers>(e => e.UserA).WithMany(e => e.LinkedUserAsA).HasForeignKey(e => e.UserAId),
                    r => r.HasOne<DbUsers>(e => e.UserB).WithMany(e => e.LinkedUserAsB).HasForeignKey(e => e.UserBId));

            //UserFunctions
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(en => en.Name).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(en => en.Order).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(en => en.CustomerId).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>().HasOne(p => p.Customer).WithMany(g => g.UserFunctions).HasForeignKey(s => s.CustomerId).IsRequired();

            //UserRoles
            modelBuilder.Entity<DbUserRoles>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserRoles>().HasOne(p => p.Customer).WithMany(g => g.UserRoles).HasForeignKey(s => s.CustomerId).IsRequired();

            //UserDefaultGroup
            modelBuilder.Entity<DbUserDefaultGroup>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserDefaultGroup>().HasOne(p => p.Customer).WithMany(g => g.UserDefaultGroups).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserDefaultGroup>().HasOne(p => p.User).WithMany(g => g.UserDefaultGroups).HasForeignKey(s => s.UserId).IsRequired();

            //UserDefaultAvailables
            modelBuilder.Entity<DbUserDefaultAvailable>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.Customer).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.User).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.RoosterDefault).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.RoosterDefaultId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.DefaultGroup).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.DefaultGroupId);

            //UserHolidays
            modelBuilder.Entity<DbUserHolidays>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserHolidays>().HasOne(p => p.Customer).WithMany(g => g.UserHolidays).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserHolidays>().HasOne(p => p.User).WithMany(g => g.UserHolidays).HasForeignKey(s => s.UserId).IsRequired();

            //UserOnVersions
            modelBuilder.Entity<DbUserOnVersion>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserOnVersion>().HasOne(p => p.Customer).WithMany(g => g.UserOnVersions).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserOnVersion>().HasOne(p => p.User).WithMany(g => g.UserOnVersions).HasForeignKey(s => s.UserId).IsRequired();

            //UserSettings
            modelBuilder.Entity<DbUserSettings>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserSettings>().HasOne(p => p.Customer).WithMany(g => g.UserSettings).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserSettings>().HasOne(p => p.User).WithMany(g => g.UserSettings).HasForeignKey(s => s.UserId).IsRequired();

            //UserLinkedMails
            modelBuilder.Entity<DbUserLinkedMails>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserLinkedMails>().HasOne(p => p.Customer).WithMany(g => g.UserLinkedMails).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserLinkedMails>().HasOne(p => p.User).WithMany(g => g.UserLinkedMails).HasForeignKey(s => s.UserId).IsRequired();
            
            //UserLogins
            modelBuilder.Entity<DbUserLogins>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbUserLogins>().HasOne(p => p.User).WithMany(g => g.Logins).HasForeignKey(s => s.UserId);
            modelBuilder.Entity<DbUserLogins>().HasOne(p => p.SharedAction).WithMany(g => g.Logins).HasForeignKey(s => s.SharedActionId);
            
            //Menu
            modelBuilder.Entity<DbMenu>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbMenu>().HasOne(p => p.Customer).WithMany(g => g.Menus).HasForeignKey(s => s.CustomerId);
            modelBuilder.Entity<DbMenu>().HasOne(p => p.Parent).WithMany(g => g.Children).HasForeignKey(s => s.ParentId);

            // Rooster available
            modelBuilder.Entity<DbRoosterAvailable>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Training).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.TrainingId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Customer).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.User).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.UserFunction).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.UserFunctionId);
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Vehicle).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.VehicleId);
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.LinkExchange).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.LinkExchangeId);

            // Rooster default
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(en => en.ValidFrom).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(en => en.ValidUntil).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(en => en.ShowTime).HasDefaultValue(true); });
            modelBuilder.Entity<DbRoosterDefault>().HasOne(p => p.RoosterTrainingType).WithMany(g => g.RoosterDefaults).HasForeignKey(s => s.RoosterTrainingTypeId);
            modelBuilder.Entity<DbRoosterDefault>().HasOne(p => p.Customer).WithMany(g => g.RoosterDefaults).HasForeignKey(s => s.CustomerId).IsRequired();

            //Rooster item day
            modelBuilder.Entity<DbRoosterItemDay>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterItemDay>().HasOne(p => p.Customer).WithMany(g => g.RoosterItemDays).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbRoosterItemDay>().HasMany(p => p.Users).WithMany(g => g.RoosterItemDays)
                .UsingEntity<DbLinkUserDayItem>(
                    l => l.HasOne<DbUsers>(e => e.User).WithMany(e => e.LinkUserDayItems).HasForeignKey(e => e.UserId),
                    r => r.HasOne<DbRoosterItemDay>(e => e.DayItem).WithMany(e => e.LinkUserDayItems).HasForeignKey(e => e.DayItemId));

            //Rooster item month
            modelBuilder.Entity<DbRoosterItemMonth>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterItemMonth>().HasOne(p => p.Customer).WithMany(g => g.RoosterItemMonths).HasForeignKey(s => s.CustomerId).IsRequired();

            // Rooster training
            modelBuilder.Entity<DbRoosterTraining>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterTraining>(e => { e.Property(en => en.ShowTime).HasDefaultValue(true); });
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.Customer).WithMany(g => g.RoosterTrainings).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.RoosterDefault).WithMany(g => g.RoosterTrainings).HasForeignKey(s => s.RoosterDefaultId);
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.RoosterTrainingType).WithMany(g => g.RoosterTrainings).HasForeignKey(s => s.RoosterTrainingTypeId);

            // Rooster training type
            modelBuilder.Entity<DbRoosterTrainingType>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterTrainingType>().HasOne(p => p.Customer).WithMany(g => g.RoosterTrainingTypes).HasForeignKey(s => s.CustomerId).IsRequired();

            // Vehicles
            modelBuilder.Entity<DbVehicles>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbVehicles>().HasOne(p => p.Customer).WithMany(g => g.Vehicles).HasForeignKey(s => s.CustomerId).IsRequired();

            //PreComForward
            modelBuilder.Entity<DbPreComForward>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbPreComForward>().HasOne(p => p.User).WithMany(g => g.PreComForwards).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbPreComForward>().HasOne(p => p.Customer).WithMany(g => g.PreComForwards).HasForeignKey(s => s.CustomerId).IsRequired();
            // PreComAlerts
            modelBuilder.Entity<DbPreComAlert>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbPreComAlert>().HasOne(p => p.User).WithMany(g => g.PreComAlerts).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbPreComAlert>().HasOne(p => p.Customer).WithMany(g => g.PreComAlerts).HasForeignKey(s => s.CustomerId).IsRequired();

            //// Reports
            // ReportActionShared
            modelBuilder.Entity<DbReportActionShared>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbReportActionShared>().HasOne(p => p.Customer).WithMany(g => g.ReportActionShares).HasForeignKey(s => s.CustomerId).IsRequired();

            // ReportActions
            modelBuilder.Entity<DbReportAction>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbReportAction>().HasMany(p => p.Users);
            modelBuilder.Entity<DbReportAction>().HasOne(p => p.Customer).WithMany(g => g.ReportActions).HasForeignKey(s => s.CustomerId).IsRequired();

            // ReportTrainings
            modelBuilder.Entity<DbReportTraining>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbReportTraining>().HasMany(p => p.Users);
            modelBuilder.Entity<DbReportTraining>().HasOne(p => p.Customer).WithMany(g => g.ReportTrainings).HasForeignKey(s => s.CustomerId).IsRequired();

            // ReportUsers
            modelBuilder.Entity<DbReportUser>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbReportUser>().HasOne(p => p.Action).WithMany(g => g.Users).HasForeignKey(s => s.DbReportActionId);
            modelBuilder.Entity<DbReportUser>().HasOne(p => p.Training).WithMany(g => g.Users).HasForeignKey(s => s.DbReportTrainingId);

            //// Links
            // Vehicles <--> Rooster available
            modelBuilder.Entity<DbLinkVehicleTraining>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbLinkVehicleTraining>().HasOne(p => p.Customer).WithMany(g => g.LinkVehicleTrainings).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbLinkVehicleTraining>().HasOne(p => p.RoosterTraining).WithMany(g => g.LinkVehicleTrainings).HasForeignKey(s => s.RoosterTrainingId).IsRequired();
            modelBuilder.Entity<DbLinkVehicleTraining>().HasOne(p => p.Vehicles).WithMany(g => g.LinkVehicleTrainings).HasForeignKey(s => s.VehicleId).IsRequired();

            // Vehicle with training with outlook exchange calendar
            modelBuilder.Entity<DbLinkExchange>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbLinkExchange>().HasOne(p => p.Customer).WithMany(g => g.LinkExchanges).HasForeignKey(s => s.CustomerId).IsRequired();

            // User <--> UserRole
            modelBuilder.Entity<DbLinkUserRole>(e => { e.Property(en => en.Id).IsRequired(); });
            modelBuilder.Entity<DbLinkUserRole>().HasOne(p => p.User).WithMany(g => g.LinkUserRoles).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbLinkUserRole>().HasOne(p => p.Role).WithMany(g => g.LinkUserRoles).HasForeignKey(s => s.RoleId).IsRequired();

            // Required data
            SetCustomer(modelBuilder);
            SetDefaultRooster(modelBuilder);
            SetRoosterItemsMonth(modelBuilder);
            SetUserFunctions(modelBuilder);
            SetUserRoles(modelBuilder);
            SetVehicles(modelBuilder);
            SetRoosterTrainingTypes(modelBuilder);
            SetSystemUsers(modelBuilder);
            SetMenus(modelBuilder);
        }

        private static Guid IdTaco { get; } = new Guid("04a6b34a-c517-4fa0-87b1-7fde3ebc5461");

        #region Default data

        private void SetCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCustomers>(e =>
            {
                e.HasData(new DbCustomers
                {
                    Id = DefaultSettingsHelper.KnrmHuizenId,
                    Name = "KNRM Huizen",
                    TimeZone = "Europe/Amsterdam",
                    Created = new DateTime(2022, 10, 12, 18, 12, 5, DateTimeKind.Utc),
                    Instance = "https://login.microsoftonline.com/",
                    Domain = "hui.nu",
                    TenantId = DefaultSettingsHelper.KnrmHuizenId.ToString(), // Same as Id for KNRM Huizen, but could be different for future customers.
                    ClientIdServer = "220e1008-1131-4e82-a388-611cd773ddf8",
                    ClientSecretServer = "", //set in db
                    ClientIdLogin = "a9c68159-901c-449a-83e0-85243364e3cc",
                    ClientSecretLogin = "", //set in db
                });
            });
        }

        private void SetDefaultRooster(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Monday,
                    TimeStart = new TimeOnly(19, 30),
                    TimeEnd = new TimeOnly(21, 30),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 10,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = DefaultSettingsHelper.DefaultRoosterTuesday,
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Tuesday,
                    TimeStart = new TimeOnly(19, 30),
                    TimeEnd = new TimeOnly(21, 30),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 20,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Wednesday,
                    TimeStart = new TimeOnly(19, 30),
                    TimeEnd = new TimeOnly(21, 30),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 30,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Thursday,
                    TimeStart = new TimeOnly(19, 30),
                    TimeEnd = new TimeOnly(21, 30),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 40,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("860ec129-6b99-4286-b90a-a2d536377f7c"),
                    Name = "In overleg",
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Friday,
                    TimeStart = new TimeOnly(15, 00),
                    TimeEnd = new TimeOnly(17, 30),
                    ValidFrom = new DateTime(2023, 9, 21, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.Oefening1op1Id,
                    TimeZone = "Europe/Amsterdam",
                    ShowTime = false,
                    CountToTrainingTarget = false,
                    Order = 50,
                });
                ;
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = DefaultSettingsHelper.DefaultRoosterSaturdayMorning,
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Saturday,
                    TimeStart = new TimeOnly(10, 00),
                    TimeEnd = new TimeOnly(13, 00),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 60,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Saturday,
                    TimeStart = new TimeOnly(13, 00),
                    TimeEnd = new TimeOnly(16, 00),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 70,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Sunday,
                    TimeStart = new TimeOnly(10, 00),
                    TimeEnd = new TimeOnly(13, 00),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 80,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Sunday,
                    TimeStart = new TimeOnly(13, 00),
                    TimeEnd = new TimeOnly(16, 00),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                    TimeZone = "Europe/Amsterdam",
                    Order = 90,
                });
            });
        }

        private void SetRoosterItemsMonth(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("857e2ee9-8f2f-407e-9ec8-a0eaa853b957"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; GEEN",
                Month = 1,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("d7d80ee0-0e73-426f-84b2-2040057c2f7a"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "Geen ingeroosterde oefeningen in verband met het winterseizoen",
                Month = 1,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Info,
                Order = 1
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("5208deef-4529-4a30-a00e-22737cf52183"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp;  Algemene kennis & Communicatie",
                Month = 2,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("36e33dc3-8bb8-4096-a127-c3ee04a0e694"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; SAR & Hulpverlening",
                Month = 3,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("01a17983-9bbe-4bfc-b152-f73c1869393d"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; Veiligheid",
                Month = 4,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("f9d140f0-58fa-4c9a-a845-0eb5bad2814f"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; Navigatie",
                Month = 5,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("4a009dd3-db02-4668-bbb0-9a9298c23d58"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; EHBO & Procedures",
                Month = 6,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("e4c00e6b-14d5-4609-bff3-6a6533557a0b"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; Techiek & Varen",
                Month = 7,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("7696579c-403c-4a98-b30e-b19f1e90ffd0"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "Geen ingeroosterde oefeningen in verband met het hoogseizoen",
                Month = 7,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Info,
                Order = 1
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("48045049-e9cb-4d86-8f34-85578f015f76"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; ntb",
                Month = 8,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("4100bf4d-368e-47d1-b652-8fec191b4934"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "Geen ingeroosterde oefeningen in verband met het hoogseizoen",
                Month = 8,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Info,
                Order = 1
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("532b68c3-328f-45a2-8a7e-fdf7f9eee111"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; Algemene kennis",
                Month = 9,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("40cbe7bc-4ed4-4897-8cb9-357785cb58c9"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; Communicatie",
                Month = 10,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("ca0fe95d-3b84-4136-bcee-ce080228c324"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; SAR en Hulpverlening",
                Month = 11,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
            modelBuilder.Entity<DbRoosterItemMonth>(e => e.HasData(new DbRoosterItemMonth
            {
                Id = new Guid("b858e7fc-02ea-4a2a-a49d-f55c3a912c9c"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "KNRM Kompas onderwerp; Navigatie",
                Month = 12,
                Year = null,
                Type = CalendarItemType.Custom,
                Severity = Severity.Normal,
                Order = 0
            }));
        }

        private void SetUserFunctions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = DefaultSettingsHelper.KompasLeiderId,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Kompas leider",
                Order = 10,
                IsActive = true,
                TrainingOnly = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                RoleId = new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Schipper",
                Order = 20,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                RoleId = new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Schipper I.O.",
                Order = 30,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                RoleId = new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Opstapper",
                Order = 60,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                RoleId = new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Aankomend opstapper",
                Order = 70,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                RoleId = new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Opstapper op proef",
                Order = 80,
                IsDefault = true,
                IsActive = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                RoleId = new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "HRB Aankomend opstapper",
                Order = 100,
                IsActive = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Waarnemer",
                Order = 180,
                IsActive = true,
                TrainingOnly = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("0a0a2c2d-15c7-4205-93a2-621de3c30db1"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Extra",
                Order = 300,
                IsActive = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("a23f1f39-275e-4e21-901f-4878b73d3ede"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Inactief",
                Order = 400,
                TrainingTarget = 0,
                IsActive = false
            }));
        }

        private void SetUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("287359b1-2035-435b-97b0-eb260dc497d6"),
                ExternalId = "287359b1-2035-435b-97b0-eb260dc497d6",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Admin",
                Accesses =
                    $"{AccessesNames.AUTH_configure_training_types},{AccessesNames.AUTH_users_settigns},{AccessesNames.AUTH_scheduler_dayitem},{AccessesNames.AUTH_scheduler_monthitem},{AccessesNames.AUTH_scheduler_history}",
                Order = 10,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                ExternalId = "f6b0c571-9050-40d6-bf58-807981e5ed6e",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Scheduler",
                Accesses =
                    $"{AccessesNames.AUTH_scheduler},{AccessesNames.AUTH_scheduler_in_table_view},{AccessesNames.AUTH_scheduler_edit_past},{AccessesNames.AUTH_scheduler_dayitem},{AccessesNames.AUTH_scheduler_other_user},{AccessesNames.AUTH_scheduler_monthitem}",
                Order = 20,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("90a40128-183f-408b-aa64-eb3b279a7042"),
                ExternalId = "90a40128-183f-408b-aa64-eb3b279a7042",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Basic scheduler",
                Accesses = $"{AccessesNames.AUTH_scheduler_other_user}",
                Order = 30,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                ExternalId = "d72ed2e9-911e-4ee5-b07e-cbd5917d432b",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Users admin",
                Accesses = $"{AccessesNames.AUTH_users_counter},{AccessesNames.AUTH_users_details},{AccessesNames.AUTH_training_history_full},{AccessesNames.AUTH_action_history_full}",
                Order = 40,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("d526e5ed-e838-499d-a96c-62180db28bed"),
                ExternalId = "d526e5ed-e838-499d-a96c-62180db28bed",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Beta user",
                Accesses = $"{AccessesNames.AUTH_dashboard_Statistics}",
                Order = 50,
            }));

            //roles
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("f5b0bab6-6fdf-457d-855d-bbea6ea57bd5"),
                ExternalId = "f5b0bab6-6fdf-457d-855d-bbea6ea57bd5",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "schipper",
                Accesses = $"{AccessesNames.AUTH_training_history_full},{AccessesNames.AUTH_action_history_full},{AccessesNames.AUTH_scheduler_other_user}",
                Order = 60,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("54aace50-0e1f-4c35-a1b3-87c9ff6bd743"),
                ExternalId = "54aace50-0e1f-4c35-a1b3-87c9ff6bd743",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "schipper io",
                Accesses = $"{AccessesNames.AUTH_training_history_full},{AccessesNames.AUTH_action_history_full},{AccessesNames.AUTH_scheduler_other_user}",
                Order = 70,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("afb45395-89ee-413d-9385-21962772dbda"),
                ExternalId = "afb45395-89ee-413d-9385-21962772dbda",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "opstapper",
                Accesses = $"{AccessesNames.AUTH_training_history_full},{AccessesNames.AUTH_action_history_full},{AccessesNames.AUTH_scheduler_other_user}",
                Order = 80,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("2197a054-e81f-4720-9f08-321377398cb6"),
                ExternalId = "2197a054-e81f-4720-9f08-321377398cb6",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "aankomend opstapper",
                Accesses = $"{AccessesNames.AUTH_training_history_full},{AccessesNames.AUTH_action_history_full},{AccessesNames.AUTH_scheduler_other_user}",
                Order = 90,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("f06a00e3-62c9-4ba5-baea-84a5ba10f53a"),
                ExternalId = "f06a00e3-62c9-4ba5-baea-84a5ba10f53a",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "hrb aankomend opstapper",
                Accesses = $"{AccessesNames.AUTH_training_history_full},{AccessesNames.AUTH_action_history_full}",
                Order = 100,
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("2956c6f9-6b83-46eb-8890-dbb640fd5023"),
                ExternalId = "2956c6f9-6b83-46eb-8890-dbb640fd5023",
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "opstapper op proef",
                Accesses = $"",
                Order = 110,
            }));
        }

        private void SetVehicles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = DefaultSettingsHelper.NwiId,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                ExchangeId = new Guid("dbaeaa44-d318-464e-ac39-f85029dd9e8f"),
                Name = "Nikolaas Wijsenbeek",
                Code = "NWI",
                IsDefault = true,
                IsActive = true,
                Order = 10,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                ExchangeId = new Guid("731fa301-d7cc-41de-9063-f86a32c2b25b"),
                Name = "De Huizer",
                Code = "HZR",
                IsActive = true,
                Order = 20,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Vlet",
                Code = "HZN018",
                IsActive = true,
                Order = 30,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("d2b920a5-e8ec-4d47-a280-8f88eae914c1"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Pieter Houbolt",
                Code = "Piet",
                IsActive = true,
                Order = 40,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Wal",
                Code = "Wal",
                IsActive = true,
                Order = 100,
            }));
        }

        private void SetRoosterTrainingTypes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = DefaultSettingsHelper.KompasOefeningId,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Kompas oefening",
                ColorLight = "#bdbdbdff",
                ColorDark = "#ffffff4c",
                Order = 10,
                CountToTrainingTarget = true,
                IsDefault = true,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "EHBO",
                ColorLight = "rgb(214,129,0)",
                ColorDark = "rgb(214,143,0)",
                Order = 20,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = DefaultSettingsHelper.Oefening1op1Id,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "een op een",
                ColorLight = "rgb(25,169,140)",
                ColorDark = "",
                Order = 30,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Brandweer",
                ColorLight = "rgb(242,28,13)",
                ColorDark = "rgb(244,47,70)",
                TextColorLight = "#FFFFFF",
                TextColorDark = "#C0C0C0",
                Order = 40,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "HRB oefening",
                ColorLight = "rgb(0,235,98)",
                ColorDark = "rgb(13,222,156)",
                Order = 50,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Evenement",
                ColorLight = "#ADD8E6",
                ColorDark = "#3BB9FF",
                Order = 60,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Techniek",
                ColorLight = "#919454",
                ColorDark = "#5f6138",
                Order = 70,
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("68be785c-1226-4280-a110-bd87f328951f"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Proeve van Bekwaamheid",
                ColorLight = "#000000",
                Order = 80,
                TextColorLight = "#FFFFFF",
                TextColorDark = "#C0C0C0",
                CreatedBy = IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
        }

        private void SetSystemUsers(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUsers>(e => e.HasData(new DbUsers
            {
                Id = DefaultSettingsHelper.SystemUser,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "System",
                Email = "system@drogecode.nl",
                IsSystemUser = true,
                CreatedOn = new DateTime(2024, 10, 11, 19, 46, 12, DateTimeKind.Utc),
            }));
        }
        
        private void SetMenus(ModelBuilder modelBuilder)
        {
            var parent = new Guid("2bf106c9-eae7-4a0d-978d-54af6c4e96a1");
            modelBuilder.Entity<DbMenu>(e => e.HasData(new DbMenu
            {
                Id = parent,
                ParentId = null,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "Handige links",
                Url = "",
                IsGroup = true,
                AddLoginHint = null,
                TargetBlank = false,
                Order = 10,
            }));
            modelBuilder.Entity<DbMenu>(e => e.HasData(new DbMenu
            {
                Id = new Guid("af84e214-7def-45ac-95c9-c8a66d1573a2"),
                ParentId = parent,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "Sharepoint",
                Url = "https://dorus1824.sharepoint.com",
                IsGroup = false,
                AddLoginHint = '?',
                TargetBlank = true,
                Order = 20,
            }));
            modelBuilder.Entity<DbMenu>(e => e.HasData(new DbMenu
            {
                Id = new Guid("953de109-5526-433b-8dc8-61b10fa8fd20"),
                ParentId = parent,
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Text = "LPLH",
                Url = "https://dorus1824.sharepoint.com/:b:/r/sites/KNRM/Documenten/EHBO/LPLH/20181115%20LPLH_KNRM_1_1.pdf?csf=1&web=1&e=4L3VPo",
                IsGroup = false,
                AddLoginHint = '&',
                TargetBlank = true,
                Order = 30,
            }));
        }
        
        #endregion
    }
}