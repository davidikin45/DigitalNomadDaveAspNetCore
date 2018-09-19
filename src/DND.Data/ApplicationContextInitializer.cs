using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Tasks;
using DND.Data.Initializers;

namespace DND.Data
{
    public class ApplicationContextInitializer : IRunOnWebHostStartup
    {
        private IDbContextFactoryProducerSingleton _dbContextFactory;
        public ApplicationContextInitializer(IDbContextFactoryProducerSingleton dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<ApplicationContext>().CreateDbContext())
            {
                var migrationInitializer = new ApplicationContextInitializerMigrate(dbContext);
                migrationInitializer.Initialize();
            }
        }
    }
}
