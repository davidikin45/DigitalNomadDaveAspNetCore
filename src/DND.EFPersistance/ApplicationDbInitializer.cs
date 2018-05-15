using DND.EFPersistance.Initializers;
using DND.Common.Implementation.Persistance;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Persistance;
using DND.Common.Tasks;

namespace DND.EFPersistance
{
    public class ApplicationDbInitializer : IRunAtStartup
    {
        private IDbContextFactory _dbContextFactory;
        public ApplicationDbInitializer(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute()
        {
            //DbContextInitializer<ApplicationDbContext>.SetInitializer(new ApplicationDbInitializerDropCreate(), true, true);

            if(bool.Parse(ConnectionStrings.GetConnectionString("UseSQLite")))
            {
                DbContextInitializer<ApplicationDbContext>.SetInitializer(_dbContextFactory, new ApplicationDbInitializerMigrateSQLite(), true, true);
            }
            else
            {
                DbContextInitializer<ApplicationDbContext>.SetInitializer(_dbContextFactory, new ApplicationDbInitializerMigrate(), true, true);
            }
        }
    }
}
