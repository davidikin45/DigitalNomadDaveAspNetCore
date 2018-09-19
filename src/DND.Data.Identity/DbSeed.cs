using DND.Common.Infrastructure;
using DND.Domain.Identity.Users;
using DND.Infrastructure;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace DND.Data.Identity
{
    public class DbSeed
    {
        public static void Seed(IdentityContext context)
        {
            CreateUsers(context);
        }

        private static void CreateUsers(IdentityContext context)
        {
            if (!context.Users.Any())
            {
                var adminRole = new IdentityRole() { Id = Guid.NewGuid().ToString(), Name = DNDSeedData.AdminRoleName };
                adminRole.NormalizedName = adminRole.Name.ToUpper();
                context.Roles.Add(adminRole);

                var password = DNDSeedData.AdminPassword;

                var passwordHasher = new PasswordHasher<User>();
                var adminUser = new User { Id = Guid.NewGuid().ToString(), UserName = DNDSeedData.AdminUsername, Name = DNDSeedData.AdminName, Email = DNDSeedData.AdminEmail, EmailConfirmed = true, LockoutEnabled = true, SecurityStamp = "InitialSecurityStamp" };
                adminUser.NormalizedUserName = adminUser.UserName.ToUpper();
                adminUser.NormalizedEmail = adminUser.Email.ToUpper();
                adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, password);

                context.Users.Add(adminUser);

                context.UserRoles.Add(new IdentityUserRole<string>() { UserId = adminUser.Id, RoleId = adminRole.Id });

                var claim = new IdentityUserClaim<string>() { ClaimType = "scope", ClaimValue = ApiScopes.Full, UserId = adminUser.Id };
                context.UserClaims.Add(claim);
            }
        }

    }
}
