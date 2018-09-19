using DND.Common.Infrastructure.Interfaces.Data;
using Microsoft.EntityFrameworkCore;


namespace DND.Common.Testing
{
    public class FakeSingleDbContextFactory<TDbContext> : IDbContextFactory<TDbContext> where TDbContext : DbContext
    {
        private DbContext _dbContext;
        public FakeSingleDbContextFactory(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbContext CreateBaseDbContext()
        {
            return _dbContext;
        }

        public TDbContext CreateDbContext()
        {
            return (TDbContext)_dbContext;
        }
    }
}
