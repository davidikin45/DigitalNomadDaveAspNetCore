using DND.Common.DomainEvents;
using DND.Common.Interfaces.Data;
using DND.Infrastructure;
using DND.Interfaces.Blog.Data;
using DND.Interfaces.CMS.Data;
using DND.Interfaces.Data;
using System.Data.SQLite;

namespace DND.Data
{
    public class ApplicationDbContextFactory : IDbContextFactory<ICMSDbContext>, IDbContextFactory<IBlogDbContext>, IDbContextFactory<ApplicationDbContext>, IDbContextFactory<IApplicationDbContext>
    {
        private IDomainEvents _domainEvents;

        public ApplicationDbContextFactory(IDomainEvents domainEvents = null)
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
                return new ApplicationDbContext(con);
            }
            else
            {
                return new ApplicationDbContext(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"), false, _domainEvents);
            }
        }

        ICMSDbContext IDbContextFactory<ICMSDbContext>.CreateDbContext()
        {
            return (ICMSDbContext)CreateBaseDbContext();
        }

        IBlogDbContext IDbContextFactory<IBlogDbContext>.CreateDbContext()
        {
            return (IBlogDbContext)CreateBaseDbContext();
        }

        public TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            return (TIDbContext)CreateBaseDbContext();
        }

        public ApplicationDbContext CreateDbContext()
        {
            return (ApplicationDbContext)CreateBaseDbContext();
        }

        IApplicationDbContext IDbContextFactory<IApplicationDbContext>.CreateDbContext()
        {
            return (IApplicationDbContext)CreateBaseDbContext();
        }
    }
}
