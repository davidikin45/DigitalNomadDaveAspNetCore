using DND.Common.Testing;
using DND.Data;
using DND.Data.Identity;
using DND.Data.Identity.Initializers;
using DND.Data.Initializers;
using Microsoft.Extensions.Configuration;
using System;

namespace DND.TestSetup
{
    public abstract class DbSetupFixture : IntegrationTestDbSetupBase<DbSetupFixture>, IDisposable
    {
        public DbSetupFixture()
             : base(TestHelper.GetConfiguration("Integration").GetConnectionString("DefaultConnection"))
        {
        }

        public override void MigrateDatabaseAndSeed(string connectionString)
        {
            using (var context = TestHelper.GetContext<ApplicationContext>(connectionString, false))
            {
                var migration = new ApplicationContextInitializerMigrate(context);
                migration.Initialize();
            }

            using (var context = TestHelper.GetContext<IdentityContext>(connectionString, false))
            {
                var migration = new IdentityContextInitializerMigrate(context);
                migration.Initialize();
            }
        }
    }
}
