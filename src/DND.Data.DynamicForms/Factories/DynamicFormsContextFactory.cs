using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data;
using DND.Data.DynamicForms;
using DND.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DND.Data
{
    public class DynamicFormsContextFactory : IDbContextFactory<DynamicFormsContext>
    {
        public DynamicFormsContextFactory()
        {
        }

        public DbContext CreateBaseDbContext()
        {
            var builder = new DbContextOptionsBuilder<DynamicFormsContext>();
            builder.UseSqlServer(ConnectionStrings.GetConnectionString("DefaultConnection"),
                options => options.EnableRetryOnFailure());

            //EF Core Doesn't support Ambient Transactions
            builder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            return new DynamicFormsContext(builder.Options);
        }

        public DynamicFormsContext CreateDbContext()
        {
            return (DynamicFormsContext)CreateBaseDbContext();
        }

    }
}
