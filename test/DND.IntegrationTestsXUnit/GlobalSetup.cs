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
            :base(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"))
        {
            OneTimeSetup();
        }

        public override void MigrateDatabaseAndSeed()
        {
            DbContextInitializer<ApplicationDbContext>.SetInitializer(new DbContextFactory(), new ApplicationDbInitializerMigrate(), true, true);

            using (var context = GivenIdentityContext(false))
            {
                context.Database.Migrate();
                context.Seed();
                context.SaveChanges();
            }
        }

        public static ApplicationIdentityDbContext GivenIdentityContext(bool beginTransaction = true)
        {
            var db = new ApplicationIdentityDbContextFactory().CreateDbContext(null);
            if (beginTransaction)
            {
                db.Database.BeginTransaction(); //For EF Core need to use this instead of Isolated attribute.
            }
            return db;
        }
    }
}
