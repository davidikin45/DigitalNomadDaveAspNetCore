using DND.Common.Data;
using DND.Data.DynamicForms;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DND.Data
{
    public class DynamicFormsContextDesignTimeFactory : DesignTimeDbContextFactoryBase<DynamicFormsContext>
    {
        public DynamicFormsContextDesignTimeFactory()
            : base("DefaultConnectionString", typeof(DynamicFormsContext).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override DynamicFormsContext CreateNewInstance(DbContextOptions<DynamicFormsContext> options)
        {
            return new DynamicFormsContext(options);
        }
    }
}
