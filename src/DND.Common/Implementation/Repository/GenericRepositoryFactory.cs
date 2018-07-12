using DND.Common.Implementation.Repository.EntityFramework;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.UnitOfWork;
using System.Threading;

namespace DND.Common.Implementation.Repository
{
    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        public virtual IGenericEFRepository<TEntity> Get<TEntity>(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
        {
            return new GenericEFRepository<TEntity>(context, uow, cancellationToken);
        }

        public virtual IGenericEFReadOnlyRepository<TEntity> GetReadOnly<TEntity>(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, new()
        {
            return new GenericEFReadOnlyRepository<TEntity>(context, uow, cancellationToken);
        }
    }
}
