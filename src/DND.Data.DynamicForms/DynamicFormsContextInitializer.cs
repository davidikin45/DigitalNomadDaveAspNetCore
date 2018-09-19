using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Tasks;
using DND.Data.DynamicForms;
using DND.Data.DynamicForms.Initializers;

namespace DND.Data
{
    public class DynamicFormsContextInitializer : IRunOnWebHostStartup
    {
        private IDbContextFactoryProducerSingleton _dbContextFactory;
        public DynamicFormsContextInitializer(IDbContextFactoryProducerSingleton dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<DynamicFormsContext>().CreateDbContext())
            {
                var migrationInitializer = new DynamicFormsContextInitializerMigrate(dbContext);
                migrationInitializer.Initialize();
            }
        }
    }
}
