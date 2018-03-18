using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.DomainServices
{
    public abstract class BaseEntityDomainService<TContext, TEntity> : BaseEntityReadOnlyDomainService<TContext, TEntity>, IBaseEntityDomainService<TEntity>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, new()
    {

        public BaseEntityDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        public virtual TEntity Create(TEntity entity)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Create(entity, "");
                unitOfWork.Complete();

                return entity;
            }
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Create(entity, "");
                await unitOfWork.CompleteAsync(cancellationToken);

                return entity;
            }
        }

        public virtual void Update(TEntity entity)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Update(entity, "");
                unitOfWork.Complete();
            }
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Update(entity, "");
                await unitOfWork.CompleteAsync(cancellationToken);
            }
        }

        public virtual void Delete(object id)
        {
            TEntity entity = GetById(id);
            Delete(entity);
        }

        public virtual async Task DeleteAsync(object id, CancellationToken cancellationToken)
        {
            TEntity entity = await GetByIdAsync(id,cancellationToken);
            await DeleteAsync(entity, cancellationToken);
        }

        public virtual void Delete(TEntity entity)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Delete(entity);
                unitOfWork.Complete();
            }
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Delete(entity);
                await unitOfWork.CompleteAsync(cancellationToken);
            }
        }

    }
}
