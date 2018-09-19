using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Tasks;
using DND.Data.Identity.Initializers;

namespace DND.Data.Identity
{
    public class IdentityContextInitializer : IRunOnWebHostStartup
    {
        private IDbContextFactoryProducerSingleton _dbContextFactory;
        public IdentityContextInitializer(IDbContextFactoryProducerSingleton dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<IdentityContext>().CreateDbContext())
            {
                var migrationInitializer = new IdentityContextInitializerMigrate(dbContext);
                migrationInitializer.Initialize();
            }
        }
    }
}
