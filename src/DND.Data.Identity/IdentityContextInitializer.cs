using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Tasks;
using DND.Data.Identity.Initializers;
using Microsoft.AspNetCore.Hosting;

namespace DND.Data.Identity
{
    public class IdentityContextInitializer : IRunOnWebHostStartup
    {
        private readonly IDbContextFactoryProducerSingleton _dbContextFactory;
        private readonly IHostingEnvironment _hostingEnvironment;

        public IdentityContextInitializer(IDbContextFactoryProducerSingleton dbContextFactory, IHostingEnvironment hostingEnvironment)
        {
            _dbContextFactory = dbContextFactory;
            _hostingEnvironment = hostingEnvironment;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<IdentityContext>().CreateDbContext())
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    var migrationInitializer = new IdentityContextInitializerDropCreate(dbContext);
                    migrationInitializer.Initialize();
                }
                else
                {
                    var migrationInitializer = new IdentityContextInitializerMigrate(dbContext);
                    migrationInitializer.Initialize();
                }
            }
        }
    }
}
