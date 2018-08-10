using DND.Common.DomainEvents;
using DND.Common.Interfaces.Data;
using DND.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DND.Data.Identity
{
    public class IdentityDbContextFactory : IDbContextFactory<IdentityDbContext>
{
        private IDomainEvents _domainEvents;

        public IdentityDbContextFactory(IDomainEvents domainEvents = null)
        {
            _domainEvents = domainEvents;
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
        }

        public IBaseDbContext CreateBaseDbContext()
        {
            var builder = new DbContextOptionsBuilder<IdentityDbContext>();
            builder.UseSqlServer(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"));

            //EF Core Doesn't support Ambient Transactions
            builder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            return new IdentityDbContext(builder.Options);
        }

        public IdentityDbContext CreateDbContext()
        {
            return (IdentityDbContext)CreateBaseDbContext();
        }
    }
}
