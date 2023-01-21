using Drogecode.Knrm.Oefenrooster.Database.Models;
using Drogecode.Knrm.Oefenrooster.Server.Database.Models;
using Drogecode.Knrm.Oefenrooster.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Server.Database
{
    public class DataContext : DbContext
    {
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
            // Users
            modelBuilder.Entity<DbUsers>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbUsers>(entity => { entity.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbUsers>(entity => { entity.Property(e => e.Email).IsRequired(); });
            // demo seed data
            modelBuilder.Entity<DbUsers>(entity => { entity.HasData(new DbUsers { Name = "from model creating", Id = new Guid("46a4ddb6-412b-4329-b48f-ed681c96bc26"), CustomerId = DefaultSettingsHelper.KnrmHuizenId, Email = "test@drogecode.nl", Created = new DateTime(1992, 9, 4, 6, 30, 42, DateTimeKind.Utc) }); });

            // Customers
            modelBuilder.Entity<DbCustomers>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(entity => { entity.Property(e => e.Name).IsRequired(); });
            modelBuilder.Entity<DbCustomers>(entity => { entity.Property(e => e.Created).IsRequired(); });

            // Rooster available
            modelBuilder.Entity<DbRoosterAvailable>(entity => { entity.Property(e => e.Id).IsRequired(); });
            modelBuilder.Entity<DbRoosterAvailable>().HasOne(p => p.Training).WithMany(g => g.RoosterAvailables).IsRequired();

            // Rooster training
            modelBuilder.Entity<DbRoosterTraining>(entity => { entity.Property(e => e.Id).IsRequired(); });

        }
    }
}
