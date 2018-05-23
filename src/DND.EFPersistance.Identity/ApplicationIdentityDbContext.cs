using DND.Domain.Models;
using Microsoft.EntityFrameworkCore;
using DND.Common.Implementation.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using DND.Domain;
using DND.EFPersistance.Identity.Configurations;
using DND.Common.DomainEvents;

namespace DND.EFPersistance.Identity
{
    //Set Context Project as Visual Studio StartUp Project
    //Add-Migration InitialMigration -Context ApplicationIdentityDbContext
    //Add-Migration "Name" -Context ApplicationIdentityDbContext
    //Update-Database
    //Remove-Migration
    public class ApplicationIdentityDbContext : BaseIdentityDbContext<User>
    { 
        public ApplicationIdentityDbContext(DbContextOptions options, IDomainEvents domainEvents = null)
            :base(options, domainEvents)
        {
            Database.SetCommandTimeout(180);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.Entity<User>().ToTable("User");
        }

        public void Seed()
        {
            if (Users.Any())
                return;

            var adminRole = new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = DNDSeedData.AdminRoleName };
            adminRole.NormalizedName = adminRole.Name.ToUpper();
            Roles.Add(adminRole);

            var password = DNDSeedData.AdminPassword;

            var passwordHasher = new PasswordHasher<User>();
            var adminUser = new User { Id = Guid.NewGuid().ToString(), UserName = DNDSeedData.AdminUsername, Name = DNDSeedData.AdminName, Email = DNDSeedData.AdminEmail, EmailConfirmed = true, LockoutEnabled = true };
            adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
            adminUser.NormalizedEmail = adminUser.Email.ToUpper();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, password);

            Users.Add(adminUser);

            UserRoles.Add(new IdentityUserRole<string>() { UserId = adminUser.Id, RoleId = adminRole.Id });
        }
    }
}
