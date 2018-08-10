using DND.Common.Infrastructure;
using DND.Common.Interfaces.Data;
using DND.Common.Tasks;
using DND.Data.DynamicForms.Initializers;
using DND.Data.Initializers;
using DND.Infrastructure;

namespace DND.Data.DynamicForms
{
    public class DynamicFormsDbInitializer : IRunOnWebHostStartup
    {
        private IDbContextFactoryProducerSingleton _dbContextFactory;
        public DynamicFormsDbInitializer(IDbContextFactoryProducerSingleton dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            if(bool.Parse(DNDConnectionStrings.GetConnectionString("UseSQLite")))
            {
                DbContextInitializer<DynamicFormsDbContext>.SetInitializer(_dbContextFactory, new DynamicFormsDbInitializerMigrateSQLite(), true, true);
            }
            else
            {
                DbContextInitializer<DynamicFormsDbContext>.SetInitializer(_dbContextFactory, new DynamicFormsDbInitializerMigrate(), true, true);
            }
        }
    }
}
