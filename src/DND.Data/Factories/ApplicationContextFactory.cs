using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DND.Data
{
    public class ApplicationContextFactory : IDbContextFactory<ApplicationContext>
    {
        public ApplicationContextFactory()
        {

        }

        public DbContext CreateBaseDbContext()
        {
            var builder = new DbContextOptionsBuilder<ApplicationContext>();
            builder.UseSqlServer(ConnectionStrings.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure());

            //EF Core Doesn't support Ambient Transactions
            builder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            return new ApplicationContext(builder.Options);
        }

        public ApplicationContext CreateDbContext()
        {
            return (ApplicationContext)CreateBaseDbContext();
        }

    }
}
