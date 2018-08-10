using DND.Common.DomainEvents;
using DND.Common.Interfaces.Data;
using DND.Infrastructure;
using DND.Interfaces.DynamicForms.Data;
using System.Data.SQLite;

namespace DND.Data.DynamicForms.Factories
{
    public class DynamicFormsDbContextFactory : IDbContextFactory<IDynamicFormsDbContext>, IDbContextFactory<DynamicFormsDbContext>
    {
        private IDomainEvents _domainEvents;

        public DynamicFormsDbContextFactory(IDomainEvents domainEvents = null)
        {
            _domainEvents = domainEvents;
            var connectionString = DNDConnectionStrings.GetConnectionString("DefaultConnectionString");
        }

        public IBaseDbContext CreateBaseDbContext()
        {
            if (bool.Parse(DNDConnectionStrings.GetConnectionString("UseSQLite")))
            {
                var con = new SQLiteConnection()
                {
                    ConnectionString = DNDConnectionStrings.GetConnectionString("SQLite")
                };
                return new DynamicFormsDbContext(con);
            }
            else
            {
                return new DynamicFormsDbContext(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"), false, _domainEvents);
            }
        }

        public TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            return (TIDbContext)CreateBaseDbContext();
        }

        public IDynamicFormsDbContext CreateDbContext()
        {
            return (IDynamicFormsDbContext)CreateBaseDbContext();
        }

        DynamicFormsDbContext IDbContextFactory<DynamicFormsDbContext>.CreateDbContext()
        {
            return (DynamicFormsDbContext)CreateBaseDbContext();
        }
    }
}
