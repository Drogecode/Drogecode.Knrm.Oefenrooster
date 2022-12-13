﻿using Drogecode.Knrm.Oefenrooster.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drogecode.Knrm.Oefenrooster.Server.Database
{
    public class DataContext : DbContext
    {
        public DbSet<DbUsers> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> context) : base(context)
        {
        }
    }   
}
