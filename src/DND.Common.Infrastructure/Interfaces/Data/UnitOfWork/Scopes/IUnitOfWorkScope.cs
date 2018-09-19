using DND.Common.Infrastructure.Interfaces.Data.Repository;
using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using Microsoft.EntityFrameworkCore;

namespace DND.Common.Infrastructure.Interfaces.Data.UnitOfWork
{
    public interface IUnitOfWorkScope
    {
        IGenericEFReadOnlyRepository<TEntity> ReadOnlyRepository<TContext, TEntity>()
            where TContext : DbContext
           where TEntity : class;
    }
}
