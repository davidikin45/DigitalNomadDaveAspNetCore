using Solution.Base.Implementation.Repository.EntityFramework;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.Repository;
using System.Threading;

namespace Solution.Base.Implementation.Repository
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
