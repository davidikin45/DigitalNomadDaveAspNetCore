using DND.Common.Infrastructure.Interfaces.Data;
using DND.Common.Infrastructure.Tasks;
using DND.Data.DynamicForms;
using DND.Data.DynamicForms.Initializers;
using Microsoft.Extensions.Hosting;

namespace DND.Data
{
    public class DynamicFormsContextInitializer : IRunOnWebHostStartup
    {
        private readonly IDbContextFactoryProducerSingleton _dbContextFactory;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DynamicFormsContextInitializer(IDbContextFactoryProducerSingleton dbContextFactory, IHostingEnvironment hostingEnvironment)
        {
            _dbContextFactory = dbContextFactory;
            _hostingEnvironment = hostingEnvironment;
        }

        public void Execute()
        {
            using (var dbContext = _dbContextFactory.GetFactory<DynamicFormsContext>().CreateDbContext())
            {
                if (_hostingEnvironment.IsDevelopment())
                {
                    var migrationInitializer = new DynamicFormsContextInitializerDropCreate(dbContext);
                    migrationInitializer.Initialize();
                }
                else
                {
                    var migrationInitializer = new DynamicFormsContextInitializerMigrate(dbContext);
                    migrationInitializer.Initialize();
                }
            }
        }
    }
}
