using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using System.Linq;
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

        public virtual Result<TEntity> Create(TEntity entity)
        {
            var objectValidationErrors = entity.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TEntity>(objectValidationErrors);
            }

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Create(entity, "");
                unitOfWork.Complete();

                return Result.Ok(entity);
            }
        }

        public virtual async Task<Result<TEntity>> CreateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var objectValidationErrors = entity.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TEntity>(objectValidationErrors);
            }

            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Create(entity, "");
                await unitOfWork.CompleteAsync(cancellationToken);

                return Result.Ok(entity);
            }
        }

        public virtual Result Update(TEntity entity)
        {
            var objectValidationErrors = entity.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TEntity>(objectValidationErrors);
            }

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Update(entity, "");
                unitOfWork.Complete();
            }

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            var objectValidationErrors = entity.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TEntity>(objectValidationErrors);
            }

            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Update(entity, "");
                await unitOfWork.CompleteAsync(cancellationToken);
            }

            return Result.Ok();
        }

        public virtual Result Delete(object id)
        {
            TEntity entity = GetById(id);
            return Delete(entity);
        }

        public virtual async Task<Result> DeleteAsync(object id, CancellationToken cancellationToken)
        {
            TEntity entity = await GetByIdAsync(id,cancellationToken);
            return await DeleteAsync(entity, cancellationToken);
        }

        public virtual Result Delete(TEntity entity)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Delete(entity);
                unitOfWork.Complete();
            }

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Delete(entity);
                await unitOfWork.CompleteAsync(cancellationToken);
            }

            return Result.Ok();
        }

    }
}
