using DND.EFPersistance.Initializers;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Tasks;

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
            DbContextInitializer<ApplicationDbContext>.SetInitializer(_dbContextFactory, new ApplicationDbInitializerMigrate(), true, true);
        }
    }
}
