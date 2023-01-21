using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Drogecode.Knrm.Oefenrooster.Server.Database
{
    public class DataContext : DbContext
    {
        public DbSet<DbAudit> Audits { get; set; }
        public DbSet<DbUsers> Users { get; set; }
        public DbSet<DbCustomers> Customers { get; set; }
        public DbSet<DbRoosterDefault> RoosterDefaults { get; set; }
        public DbSet<DbRoosterTraining> RoosterTrainings { get; set; }
        public DbSet<DbRoosterAvailable> RoosterAvailables { get; set; }


        public DataContext(DbContextOptions<DataContext> context) : base(context)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Audit
            modelBuilder.Entity<DbAudit>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbAudit>().HasOne(p => p.User).WithMany(g => g.Audits).HasForeignKey(s => s.UserId).IsRequired();
            modelBuilder.Entity<DbAudit>().HasOne(p => p.Customer).WithMany(g => g.Audits).HasForeignKey(s => s.CustomerId).IsRequired();

            // Customers
            modelBuilder.Entity<DbCustomers>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(entity => { entity.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(entity => { entity.Property(e => e.Created).IsRequired(); });

            // Users
            modelBuilder.Entity<DbUsers>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUsers>(entity => { entity.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbUsers>(entity => { entity.Property(e => e.Email).IsRequired(); });
            modelBuilder.Entity<DbUsers>().HasOne(p => p.Customer).WithMany(g => g.Users).HasForeignKey(s => s.CustomerId).IsRequired();

            // Rooster available
            modelBuilder.Entity<DbRoosterAvailable>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Training).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.TrainingId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Customer).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.CustomerId).IsRequired();
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.User).WithMany(g => g.RoosterAvailables).HasForeignKey(s => s.UserId).IsRequired();

            // Rooster defrault
            modelBuilder.Entity<DbRoosterDefault>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterDefault>().HasOne(p => p.Customer).WithMany(g => g.RoosterDefaults).HasForeignKey(s => s.CustomerId).IsRequired();

            // Rooster training
            modelBuilder.Entity<DbRoosterTraining>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterTraining>().HasOne(p => p.Customer).WithMany(g => g.RoosterTraining).HasForeignKey(s => s.CustomerId).IsRequired();

            // Required data
            SetCustomer(modelBuilder);
            SetDefaultRooster(modelBuilder);

        }

        #region Default data
        private void SetCustomer(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCustomers>(entity =>
            {
                entity.HasData(new DbCustomers
                {
                    Id = DefaultSettingsHelper.KnrmHuizenId,
                    Name = "KNRM Huizen",
                    Created = new DateTime(2022, 10, 12)
                });
            });
        }

        private void SetDefaultRooster(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("4142048e-82dc-4015-aab7-1b519da01238"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Monday,
                    StartTime = new TimeOnly(19, 30),
                    EndTime = new TimeOnly(21, 30)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("7b4693a8-ae9c-430f-9119-49a6ecbfeb54"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(19, 30),
                    EndTime = new TimeOnly(21, 30)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("c1967b6b-1f3b-41d2-bfa4-361a71cd064c"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Wednesday,
                    StartTime = new TimeOnly(19, 30),
                    EndTime = new TimeOnly(21, 30)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("b73bd006-0d29-4d4e-b71b-2c382d5f703f"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(19, 30),
                    EndTime = new TimeOnly(21, 30)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("3d73993f-9935-4ebc-b16d-4d444ea8e93a"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Tuesday,
                    StartTime = new TimeOnly(19, 30),
                    EndTime = new TimeOnly(21, 30)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("2bdaccc0-e9f7-40c1-ae76-d9ed66e4a978"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Saturday,
                    StartTime = new TimeOnly(10, 00),
                    EndTime = new TimeOnly(13, 00)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("348c82b5-ba0d-4d31-b242-2edc1dc669c7"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Saturday,
                    StartTime = new TimeOnly(13, 00),
                    EndTime = new TimeOnly(16, 00)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("015b9e42-e233-457e-bf26-de26c3a718ba"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Sunday,
                    StartTime = new TimeOnly(10, 00),
                    EndTime = new TimeOnly(13, 00)
                });
            });
            modelBuilder.Entity<DbRoosterDefault>(entity =>
            {
                entity.HasData(new DbRoosterDefault
                {
                    Id = new Guid("80d8ac0c-a2f7-4dc9-af57-a0ed74b7f8df"),
                    CustomerId = DefaultSettingsHelper.KnrmHuizenId,
                    WeekDay = (short)DayOfWeek.Sunday,
                    StartTime = new TimeOnly(13, 00),
                    EndTime = new TimeOnly(16, 00)
                });
            });
        }
        #endregion
    }
}
