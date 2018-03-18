using DND.Domain.Interfaces.Persistance;
using DND.Common.Implementation.Persistance;
using DND.Common.Interfaces.Persistance;
using System;
using System.Data.SQLite;

namespace DND.EFPersistance
{
    public class DbContextFactory : IDbContextFactory
    {
        public DbContextFactory()
        {
        }

        public IBaseDbContext CreateDefault()
        {
            if (bool.Parse(ConnectionStrings.GetConnectionString("UseSQLite")))
            {
                var con = new SQLiteConnection()
                {
                    ConnectionString = ConnectionStrings.GetConnectionString("SQLite")
                };
                return new ApplicationDbContext(con);
            }
            else
            {
                return new ApplicationDbContext(ConnectionStrings.GetConnectionString("DefaultConnectionString"));
            }
        }

        public TIDbContext Create<TIDbContext>() where TIDbContext : IBaseDbContext
        {
            if (typeof(TIDbContext) == typeof(IApplicationDbContext) || typeof(TIDbContext) == typeof(ApplicationDbContext) || typeof(TIDbContext) == typeof(IBaseDbContext))
            {
                return (TIDbContext)CreateDefault();
            }
            else
            {
                throw new ApplicationException("DbContext Type Unknown");
            }
        }
    }
}
