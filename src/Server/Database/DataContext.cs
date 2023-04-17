﻿using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
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
        public DbSet<DbUserDefaultAvailable> UserDefaultAvailables { get; set; }
        public DbSet<DbUserHolidays> UserHolidays { get; set; }
        public DbSet<DbCustomers> Customers { get; set; }
        public DbSet<DbRoosterDefault> RoosterDefaults { get; set; }
        public DbSet<DbRoosterItemDay> RoosterItemDays { get; set; }
        public DbSet<DbRoosterItemMonth> RoosterItemMonths { get; set; }
        public DbSet<DbRoosterTraining> RoosterTrainings { get; set; }
        public DbSet<DbRoosterTrainingType> RoosterTrainingTypes { get; set; }
        public DbSet<DbRoosterAvailable> RoosterAvailables { get; set; }
        public DbSet<DbVehicles> Vehicles { get; set; }

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

            //UserDefaultAvailables
            modelBuilder.Entity<DbUserDefaultAvailable>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.Customer).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.User).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbUserDefaultAvailable>().HasOne(p => p.RoosterDefault).WithMany(g => g.UserDefaultAvailables).HasForeignKey(s => s.RoosterDefaultId).IsRequired();

            //UserHolidays
            modelBuilder.Entity<DbUserHolidays>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUserHolidays>().HasOne(p => p.Customer).WithMany(g => g.UserHolidays).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbUserHolidays>().HasOne(p => p.User).WithMany(g => g.UserHolidays).HasForeignKey(s => s.UserId).IsRequired();

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

            //// Links
            // Vehicles <--> Rooster available
            modelBuilder.Entity<DbLinkVehicleTraining>(e => { e.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbLinkVehicleTraining>().HasOne(p => p.Customer).WithMany(g => g.LinkVehicleTrainings).HasForeignKey(s => s.CustomerId).IsRequired();

            // Required data
            SetCustomer(modelBuilder);
            SetDefaultRooster(modelBuilder);
            SetRoosterItems(modelBuilder);
            SetUserFunctions(modelBuilder);
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
                    Created = new DateTime(2022, 10, 12, 18, 12, 5, DateTimeKind.Utc)
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
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Tuesday,
                    TimeStart = new TimeOnly(19, 30),
                    TimeEnd = new TimeOnly(21, 30),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
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
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
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
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(e =>
            {
                e.HasData(new DbRoosterDefault
                {
                    Id = new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = DayOfWeek.Saturday,
                    TimeStart = new TimeOnly(10, 00),
                    TimeEnd = new TimeOnly(13, 00),
                    ValidFrom = new DateTime(2022, 9, 4, 0, 0, 0, DateTimeKind.Utc),
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
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
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
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
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
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
                    ValidUntil = new DateTime(2023, 12, 31, 23, 59, 59, DateTimeKind.Utc),
                    RoosterTrainingTypeId = DefaultSettingsHelper.KompasOefeningId,
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
        }

        private void SetUserFunctions(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("d23de705-d950-4833-8b94-aa531022d450"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Kompas leider",
                Order = 10,
                Active = true,
                TrainingOnly = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("48db5dd5-cb72-4365-9bf5-959691dc54f2"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Schipper",
                Order = 20,
                Active = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("cf6e6afa-8aa5-4b3d-8198-fb5e86faf53c"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Schipper I.O.",
                Order = 30,
                Active = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("35ad11b8-d3f2-4960-b1e8-d41aaccd188a"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Opstapper",
                Order = 60,
                Active = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("feb3641f-9941-4db7-a202-14263d706516"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Aankomend opstapper",
                Order = 70,
                Active = true,
                TrainingTarget = 2,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("322858f8-fd2c-4e62-b699-92c605adbbf2"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Opstapper op proef",
                Order = 80,
                Default = true,
                Active = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("5c49fc5c-25eb-48c2-a746-74ac3a030d48"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "HRB Aankomend opstapper",
                Order = 100,
                Active = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("95427da1-e4d5-442e-962a-b04ab861a2c2"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Waarnemer",
                Order = 180,
                Active = true,
                TrainingOnly = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("0a0a2c2d-15c7-4205-93a2-621de3c30db1"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Extra",
                Order = 300,
                Active = true,
                TrainingTarget = 0,
            }));
            modelBuilder.Entity<DbUserFunctions>(e => e.HasData(new DbUserFunctions
            {
                Id = new Guid("a23f1f39-275e-4e21-901f-4878b73d3ede"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Inactief",
                Order = 400,
                TrainingTarget = 0,
                Active = false
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
                Active = true,
                Order = 10,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("c759950b-8264-4521-9a6e-ff98ad358cc1"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "De Huizer",
                Code = "HZR",
                Active = true,
                Order = 20,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("5777102a-3c9e-438e-a11f-fafb5f9649b6"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Vlet",
                Code = "HZN018",
                Active = true,
                Order = 30,
            }));
            modelBuilder.Entity<DbVehicles>(e => e.HasData(new DbVehicles
            {
                Id = new Guid("f30d1856-2d26-441e-ae6d-935bb26c4852"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Wal",
                Code = "Wal",
                Active = true,
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
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("80108015-87a7-4453-a1af-d81d15fe3582"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "EHBO",
                ColorLight = "rgb(214,129,0)",
                ColorDark = "rgb(214,143,0)",
                Order = 20,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("52260d46-c748-4ffc-b94c-2baecacbfaf4"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "een op een",
                ColorLight = "rgb(25,169,140)",
                ColorDark = "",
                Order = 30,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("137f2d85-8a4f-4407-ba78-d24ea1bcc181"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Brandweer",
                ColorLight = "rgb(242,28,13)",
                ColorDark = "rgb(244,47,70)",
                Order = 40,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("be12f5d9-b6f9-45d5-bd5f-6b74d7706a53"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "HRB oefening",
                ColorLight = "rgb(0,235,98)",
                ColorDark = "rgb(13,222,156)",
                Order = 50,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("6153a297-9486-43de-91e8-22d107da2b21"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Evenement",
                ColorLight = "#63806f",
                ColorDark = "#3b4d42",
                Order = 60,
            }));
            modelBuilder.Entity<DbRoosterTrainingType>(e => e.HasData(new DbRoosterTrainingType
            {
                Id = new Guid("61646e7b-5257-4928-87fe-f1ac8ef1ef41"),
                CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                Name = "Techniek",
                ColorLight = "#919454",
                ColorDark = "#5f6138",
                Order = 70,
            }));
        }
        #endregion
    }
}
