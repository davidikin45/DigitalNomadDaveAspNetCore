using Microsoft.EntityFrameworkCore;
using System;

namespace DND.Common.Infrastructure.Interfaces.Data
{
    public interface IDbContextFactoryProducerSingleton
    {
        IDbContextAbstractFactory GetFactory(Type type);
        IDbContextFactory<TDbContext> GetFactory<TDbContext>() where TDbContext : DbContext;
        IDbContextAbstractFactory GetAbstractFactory<TIDbContext>() where TIDbContext : DbContext;

        IDbContextAbstractFactory GetFactoryByEntityType(Type entityType);
    }
}
