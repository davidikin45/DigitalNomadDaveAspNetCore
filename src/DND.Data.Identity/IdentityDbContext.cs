using DND.Common.DomainEvents;
using DND.Common.Implementation.Data;
using DND.Data.Identity.Configurations;
using DND.Domain.Identity.Users;
using DND.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DND.Data.Identity
{
    //Set Context Project as Visual Studio StartUp Project
    //Add-Migration InitialMigration -Context ApplicationIdentityDbContext
    //Add-Migration "Name" -Context ApplicationIdentityDbContext
    //Update-Database
    //Remove-Migration
    public class IdentityDbContext : BaseIdentityDbContext<User>
    { 
        public IdentityDbContext(DbContextOptions options, IDomainEvents domainEvents = null)
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
