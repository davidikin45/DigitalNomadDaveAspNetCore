using DND.Domain.Interfaces.Persistance;
using Solution.Base.Implementation.Persistance;
using Solution.Base.Interfaces.Persistance;
using System;

namespace DND.EFPersistance
{
    public class DbContextFactory : IDbContextFactory
    {
        public DbContextFactory()
        {
        }

        public IBaseDbContext CreateDefault()
        {
            return new ApplicationDbContext(ConnectionStrings.GetConnectionString("DefaultConnectionString"));
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
