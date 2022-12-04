using Drogecode.Knrm.Oefenrooster.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Database
{
    public class DataContext : DbContext
    {
        private readonly IConfiguration Configuration;
        public DbSet<Users> Users { get; set; }

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to postgres with connection string from app settings
            options.UseNpgsql(Configuration.GetConnectionString("postgresDB"));
        }

    }
}
