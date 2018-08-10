using DND.Common.Interfaces.Data;
using DND.Common.Tasks;
using DND.Data.Identity.Initializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Data.Identity
{
    public class IdentityDbInitializer : IRunOnWebHostStartup
    {
        private IDbContextFactoryProducerSingleton _dbContextFactory;
        public IdentityDbInitializer(IDbContextFactoryProducerSingleton dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<IdentityDbContext>().CreateDbContext())
            {
                var migrationInitializer = new IdentityDbContextInitializerMigrate(dbContext);
                migrationInitializer.Initialize();
            }
        }
    }
}
