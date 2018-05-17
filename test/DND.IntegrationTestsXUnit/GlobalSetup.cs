using DND.Common.Infrastructure;
using DND.Common.Testing;
using DND.Domain;
using DND.Domain.Models;
using DND.EFPersistance;
using DND.EFPersistance.Identity;
using DND.EFPersistance.Initializers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Entity;
using System.Linq;

namespace DND.IntegrationTestsXUnit
{
    public class GlobalSetup : BaseIntegrationTestSetup<GlobalSetup>, IDisposable
    {
        public GlobalSetup()
            :base("DNDIntegrationTests")
        {
            OneTimeSetup();
        }

        public override void MigrateDatabase()
        {
            DbContextInitializer<ApplicationDbContext>.SetInitializer(new DbContextFactory(), new ApplicationDbInitializerMigrate(), true, true);
            var context = new ApplicationIdentityDbContextFactory().CreateDbContext(null);
            context.Database.Migrate();

            Seed();
        }

        public void Seed()
        {
            var context = new ApplicationIdentityDbContextFactory().CreateDbContext(null);

            if (context.Users.Any())
                return;

            context.Users.Add(new User { UserName = "user1", Name = "user1", Email = "-", PasswordHash = "-", EmailConfirmed = true });
            context.Users.Add(new User { UserName = "user2", Name = "user2", Email = "-", PasswordHash = "-", EmailConfirmed = true });
            context.SaveChanges();
        }
    }
}
