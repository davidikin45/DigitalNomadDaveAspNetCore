using DND.Domain.Models;
using Microsoft.EntityFrameworkCore;
using DND.Common.Implementation.Persistance;
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

        public void Seed()
        {
            if (Users.Any())
                return;

            Users.Add(new User { UserName = "user1", Name = "user1", Email = "-", PasswordHash = "-", EmailConfirmed = true });
            Users.Add(new User { UserName = "user2", Name = "user2", Email = "-", PasswordHash = "-", EmailConfirmed = true });
        }
    }
}
