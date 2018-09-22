using DND.Common.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DND.Data.Identity
{
    public class IdentityContextDesignTimeFactory : DesignTimeDbContextFactoryBase<IdentityContext>
    {
        public IdentityContextDesignTimeFactory()
            : base("DefaultConnection", typeof(IdentityContext).GetTypeInfo().Assembly.GetName().Name)
        {
        }

        protected override IdentityContext CreateNewInstance(DbContextOptions<IdentityContext> options)
        {
            return new IdentityContext(options);
        }
    }
}
