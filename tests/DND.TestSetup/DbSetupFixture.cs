using DND.Common.Implementation.Data;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Data;
using DND.Common.Testing;
using DND.Data;
using DND.Data.Identity;
using DND.Data.Initializers;
using DND.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;

namespace DND.TestSetup
{
    public abstract class DbSetupFixture : BaseIntegrationTestDbSetup<DbSetupFixture>, IDisposable
    {
        public DbSetupFixture()
             : base(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"))
        {
        }

        public override void MigrateDatabaseAndSeed()
        {
            DbContextInitializer<ApplicationDbContext>.SetInitializer(new DbContextFactoryProducerSingleton(new IDbContextAbstractFactory[] { new ApplicationDbContextFactory()}), new ApplicationDbInitializerMigrate(), true, true);

            using (var context = CreateIdentityContext(false))
            {
                context.Database.Migrate();
                context.Seed();
                context.SaveChanges();
            }
        }

        public static IdentityDbContext CreateIdentityContext(bool beginTransaction = true)
        {
            var db = new IdentityDbContextDesignTimeFactory().CreateDbContext(null);
            if (beginTransaction)
            {
                db.Database.BeginTransaction(); //For EF Core need to use this instead of Isolated attribute.
            }
            return db;
        }
    }
}
