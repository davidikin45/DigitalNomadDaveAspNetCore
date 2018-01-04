using DND.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Solution.Base.Implementation.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.EFPersistance.Identity
{
    //Set Context Project as Visual Studio StartUp Project
    //Add-Migration InitialMigration -Context ApplicationIdentityDbContext
    //Add-Migration "Name" -Context ApplicationIdentityDbContext
    //Update-Database
    //Remove-Migration
    public class ApplicationIdentityDbContext : BaseIdentityDbContext<User>
    { 
        public ApplicationIdentityDbContext(DbContextOptions options)
            :base(options)
        {
            Database.SetCommandTimeout(180);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("User");
        }

    }
}
