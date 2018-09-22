using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Tasks;
using DND.Data.Initializers;
using Microsoft.Extensions.Hosting;

namespace DND.Data
{
    public class ApplicationContextInitializer : IRunOnWebHostStartup
    {
        private readonly IDbContextFactoryProducerSingleton _dbContextFactory;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ApplicationContextInitializer(IDbContextFactoryProducerSingleton dbContextFactory, IHostingEnvironment hostingEnvironment)
        {
            _dbContextFactory = dbContextFactory;
            _hostingEnvironment = hostingEnvironment;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<ApplicationContext>().CreateDbContext())
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    var migrationInitializer = new ApplicationContextInitializerDropCreate(dbContext);
                    migrationInitializer.Initialize();
                }
                else
                {
                    var migrationInitializer = new ApplicationContextInitializerMigrate(dbContext);
                    migrationInitializer.Initialize();
                }
            }
        }
    }
}
