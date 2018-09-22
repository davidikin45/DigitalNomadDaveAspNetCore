using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data;
using DND.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DND.Data.Identity
{
    public class IdentityDbContextFactory : IDbContextFactory<IdentityContext>
    {
        public IdentityDbContextFactory()
        {

        }

        public DbContext CreateBaseDbContext()
        {
            var builder = new DbContextOptionsBuilder<IdentityContext>();
            builder.UseSqlServer(ConnectionStrings.GetConnectionString("DefaultConnection"));

            //EF Core Doesn't support Ambient Transactions
            builder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            return new IdentityContext(builder.Options);
        }

        public IdentityContext CreateDbContext()
        {
            return (IdentityContext)CreateBaseDbContext();
        }
    }
}
