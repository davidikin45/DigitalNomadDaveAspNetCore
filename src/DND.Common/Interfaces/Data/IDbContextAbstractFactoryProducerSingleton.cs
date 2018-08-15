using System;

namespace DND.Common.Interfaces.Data
{
    public interface IDbContextFactoryProducerSingleton
    {
        IDbContextAbstractFactory GetFactory(Type type);
        IDbContextFactory<TIDbContext> GetFactory<TIDbContext>() where TIDbContext : IBaseDbContext;
        IDbContextAbstractFactory GetAbstractFactory<TIDbContext>() where TIDbContext : IBaseDbContext;

        IDbContextAbstractFactory GetFactoryByEntityType(Type entityType);
    }
}
