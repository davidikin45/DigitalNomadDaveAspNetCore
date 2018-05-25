using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.UnitOfWork;
using System.Threading;

namespace DND.Common.Implementation.Repository
{
    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        public virtual IBaseRepository<TEntity> Get<TEntity>(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
        {
            return new GenericEFRepository<TEntity>(context, uow, true, cancellationToken);
        }

        public virtual IBaseReadOnlyRepository<TEntity> GetReadOnly<TEntity>(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, new()
        {
            return new GenericEFReadOnlyRepository<TEntity>(context, uow, true, cancellationToken);
        }
    }
}
