using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Authorization;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace Drogecode.Knrm.Oefenrooster.Server.Database
{
    public class DataContext : DbContext
    {
        public DbSet<DbAudit> Audits { get; set; }
        public DbSet<DbUsers> Users { get; set; }
        public DbSet<DbUserFunctions> UserFunctions { get; set; }
        public DbSet<DbUserRoles> UserRoles { get; set; }
        public DbSet<DbUserDefaultGroup> UserDefaultGroups{ get; set; }
        public DbSet<DbUserDefaultAvailable> UserDefaultAvailables { get; set; }
        public DbSet<DbUserHolidays> UserHolidays { get; set; }
        public DbSet<DbUserSettings> UserSettings { get; set; }
        public DbSet<DbCustomers> Customers { get; set; }
        public DbSet<DbCustomerSettings> CustomerSettings { get; set; }
        public DbSet<DbRoosterDefault> RoosterDefaults { get; set; }
        public DbSet<DbRoosterItemDay> RoosterItemDays { get; set; }
        public DbSet<DbRoosterItemMonth> RoosterItemMonths { get; set; }
        public DbSet<DbRoosterTraining> RoosterTrainings { get; set; }
        public DbSet<DbRoosterTrainingType> RoosterTrainingTypes { get; set; }
        public DbSet<DbRoosterAvailable> RoosterAvailables { get; set; }
        public DbSet<DbVehicles> Vehicles { get; set; }
        public DbSet<DbPreComAlert> PreComAlerts { get; set; }

        public DbSet<DbReportAction> ReportActions { get; set; }
        public DbSet<DbReportTraining> ReportTrainings { get; set; }
        public DbSet<DbReportUser> ReportUsers { get; set; }

        public DbSet<DbLinkVehicleTraining> LinkVehicleTraining { get; set; }


        public DataContext(DbContextOptions<DataContext> context) : base(context)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Audit
            modelBuilder.Entity<DbAudit>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbAudit>().HasOne(p => p.User).WithMany(g => g.Audits).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbAudit>().HasOne(p => p.Customer).WithMany(g => g.Audits).HasForeignKey(s => s.CustomerId).IsRequired();

            // Customers
            modelBuilder.Entity<DbCustomers>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(e => { e.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(e => { e.Property(e => e.Created).IsRequired(); });

            // CustomerSettings
            modelBuilder.Entity<DbCustomerSettings>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbCustomerSettings>().HasOne(p => p.Customer).WithMany(g => g.CustomerSettings).HasForeignKey(s => s.CustomerId).IsRequired();

            // Users
            modelBuilder.Entity<DbUsers>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUsers>(e => { e.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbUsers>(e => { e.Property(e => e.Email).IsRequired(); });
            modelBuilder.Entity<DbUsers>().HasOne(p => p.Customer).WithMany(g => g.Users).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUsers>().HasOne(p => p.UserFunction).WithMany(g => g.Users).HasForeignKey(s => s.UserFunctionId);

            //UserFunctions
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(e => e.Order).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>(e => { e.Property(e => e.CustomerId).IsRequired(); });
            modelBuilder.Entity<DbUserFunctions>().HasOne(p => p.Customer).WithMany(g => g.UserFunctions).HasForeignKey(s => s.CustomerId).IsRequired();

            //UserRoles
            modelBuilder.Entity<DbUserRoles>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserRoles>().HasOne(p => p.Customer).WithMany(g => g.UserRoles).HasForeignKey(s => s.CustomerId).IsRequired();

            //UserDefaultGroup
            modelBuilder.Entity<DbUserDefaultGroup>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserDefaultGroup>().HasOne(p => p.Customer).WithMany(g => g.UserDefaultGroups).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserDefaultGroup>().HasOne(p => p.User).WithMany(g => g.UserDefaultGroups).HasForeignKey(s => s.UserId).IsRequired();

            //UserDefaultAvailables
            modelBuilder.Entity<DbUserDefaultAvailable>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.Customer).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.User).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.RoosterDefault).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.RoosterDefaultId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.DefaultGroup).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.DefaultGroupId);

            //UserHolidays
            modelBuilder.Entity<DbUserHolidays>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserHolidays>().HasOne(p => p.Customer).WithMany(g => g.UserHolidays).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserHolidays>().HasOne(p => p.User).WithMany(g => g.UserHolidays).HasForeignKey(s => s.UserId).IsRequired();

            //UserSettings
            modelBuilder.Entity<DbUserSettings>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserSettings>().HasOne(p => p.Customer).WithMany(g => g.UserSettings).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserSettings>().HasOne(p => p.User).WithMany(g => g.UserSettings).HasForeignKey(s => s.UserId).IsRequired();

            // Rooster available
            modelBuilder.Entity<DbRoosterAvailable>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Training).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.TrainingId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Customer).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.User).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.UserFunction).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.UserFunctionId);
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Vehicle).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.VehicleId);

            // Rooster default
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(e => e.ValidFrom).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>(e => { e.Property(e => e.ValidUntil).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>().HasOne(p => p.RoosterTrainingType).WithMany(g => g.RoosterDefaults).HasForeignKey(s => s.RoosterTrainingTypeId);
            modelBuilder.Entity<DbRoosterDefault>().HasOne(p => p.Customer).WithMany(g => g.RoosterDefaults).HasForeignKey(s => s.CustomerId).IsRequired();

            //Rooster item day
            modelBuilder.Entity<DbRoosterItemDay>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterItemDay>().HasOne(p => p.Customer).WithMany(g => g.RoosterItemDays).HasForeignKey(s => s.CustomerId).IsRequired();

            //Rooster item month
            modelBuilder.Entity<DbRoosterItemMonth>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterItemMonth>().HasOne(p => p.Customer).WithMany(g => g.RoosterItemMonths).HasForeignKey(s => s.CustomerId).IsRequired();

            // Rooster training
            modelBuilder.Entity<DbRoosterTraining>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.Customer).WithMany(g => g.RoosterTrainings).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.RoosterDefault).WithMany(g => g.RoosterTrainings).HasForeignKey(s => s.RoosterDefaultId);
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.RoosterTrainingType).WithMany(g => g.RoosterTrainings).HasForeignKey(s => s.RoosterTrainingTypeId);

            // Rooster training type
            modelBuilder.Entity<DbRoosterTrainingType>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterTrainingType>().HasOne(p => p.Customer).WithMany(g => g.RoosterTrainingTypes).HasForeignKey(s => s.CustomerId).IsRequired();

            // Vehicles
            modelBuilder.Entity<DbVehicles>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbVehicles>().HasOne(p => p.Customer).WithMany(g => g.Vehicles).HasForeignKey(s => s.CustomerId).IsRequired();
            // PreComAlerts
            modelBuilder.Entity<DbPreComAlert>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbPreComAlert>().HasOne(p => p.Customer).WithMany(g => g.PreComAlerts).HasForeignKey(s => s.CustomerId).IsRequired();

            //// Reports
            // ReportActions
            modelBuilder.Entity<DbReportAction>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbReportAction>().HasMany(p => p.Users);

            // ReportTrainings
            modelBuilder.Entity<DbReportTraining>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbReportTraining>().HasMany(p => p.Users);

            // ReportUsers
            modelBuilder.Entity<DbReportUser>(e => { e.Property(e => e.Id).IsRequired(); });

            //// Links
            // Vehicles <--> Rooster available
            modelBuilder.Entity<DbLinkVehicleTraining>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbLinkVehicleTraining>().HasOne(p => p.Customer).WithMany(g => g.LinkVehicleTrainings).HasForeignKey(s => s.CustomerId).IsRequired();

            // Required data
            SetCustomer(modelBuilder);
            SetDefaultRooster(modelBuilder);
            SetRoosterItems(modelBuilder);
            SetUserFunctions(modelBuilder);
            SetUserRoles(modelBuilder);
            SetVehicles(modelBuilder);
            SetRoosterTrainingTypes(modelBuilder);
        }

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
                    TenantId = "d9754755-b054-4a9c-a77f-da42a4009365", // Same as Id for KNRM Huizen, but could be different for future customers.
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
                    Name = "1:1",
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Friday,
                    TimeStart = new TimeOnly(15, 00),
                    TimeEnd = new TimeOnly(17, 30),
                    ValidFrom = new DateTime(2023, 9, 21, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2024, 6, 30, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.Oefening1op1Id,
                    TimeZone = "Europe/Amsterdam",
                    NoTime = true,
                    CountToTrainingTarget = false,
                    Order = 50,
                }); ;
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

        private void SetRoosterItems(ModelBuilder modelBuilder)
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
                Id = new Guid("d23de705-d950-4833-8b94-aa531022d450"),
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
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Schipper",
                Order = 20,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Schipper I.O.",
                Order = 30,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Opstapper",
                Order = 60,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Aankomend opstapper",
                Order = 70,
                IsActive = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
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
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Admin",
                Accesses = AccessesNames.AUTH_configure_training_types
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("f6b0c571-9050-40d6-bf58-807981e5ed6e"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Scheduler",
                Accesses = AccessesNames.AUTH_scheduler
            }));
            modelBuilder.Entity<DbUserRoles>(e => e.HasData(new DbUserRoles
            {
                Id = new Guid("d72ed2e9-911e-4ee5-b07e-cbd5917d432b"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Users",
                Accesses = $"{AccessesNames.AUTH_users_counter},{AccessesNames.AUTH_users_details}"
            }));
        }

        private void SetVehicles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("4589535c-9064-4448-bc01-3b5a00e9410d"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
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
                CreatedBy = DefaultSettingsHelper.IdTaco,
                CreatedDate = new DateTime(2023, 06, 26, 12, 12, 12, DateTimeKind.Utc),
                IsActive = true,
            }));
        }
        #endregion
    }
}
