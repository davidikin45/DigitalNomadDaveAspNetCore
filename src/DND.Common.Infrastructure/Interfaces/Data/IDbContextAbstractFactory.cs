using Microsoft.EntityFrameworkCore;

namespace DND.Common.Infrastructure.Interfaces.Data
{
    public interface IDbContextFactory<TDbContext> : IDbContextAbstractFactory where TDbContext : DbContext
    {
        TDbContext CreateDbContext();
    }

    public interface IDbContextAbstractFactory
    {
        DbContext CreateBaseDbContext();
    }
}
