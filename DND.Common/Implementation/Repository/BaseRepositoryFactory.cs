using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.Repository;
using System.Threading;

namespace DND.Common.Implementation.Repository
{
    public class BaseRepositoryFactory : IBaseRepositoryFactory
    {
        public virtual IBaseRepository<TEntity> Get<TEntity>(IBaseDbContext context, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
        {
            return new BaseEFRepository<IBaseDbContext,TEntity>(context, true, cancellationToken);
        }

        public virtual IBaseReadOnlyRepository<TEntity> GetReadOnly<TEntity>(IBaseDbContext context, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, new()
        {
            return new BaseEFReadOnlyRepository<IBaseDbContext, TEntity>(context, true, cancellationToken);
        }
    }
}
