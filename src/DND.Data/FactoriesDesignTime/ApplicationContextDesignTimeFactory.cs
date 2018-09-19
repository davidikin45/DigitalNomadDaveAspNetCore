using DND.Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DND.Data
{
    public class ApplicationContextDesignTimeFactory : DesignTimeDbContextFactoryBase<ApplicationContext>
    {
        public ApplicationContextDesignTimeFactory()
            : base("DefaultConnectionString", typeof(ApplicationContext).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override ApplicationContext CreateNewInstance(DbContextOptions<ApplicationContext> options)
        {
            return new ApplicationContext(options);
        }
    }
}
