using DND.Common.Infrastructure.DomainEvents;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Interfaces.DomainEvents;
using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.DomainServices
{
    public abstract class DomainServiceEntityBase<TContext, TEntity> : DomainServiceEntityReadOnlyBase<TContext, TEntity>, IDomainServiceEntity<TEntity>
          where TContext : DbContext
          where TEntity : class
    {

        public DomainServiceEntityBase(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        #region GetNewEntityInstance
        public virtual TEntity GetNewEntityInstance()
        {
            return (TEntity)Activator.CreateInstance(typeof(TEntity));
        }
        #endregion

        #region Create
        public virtual Result<TEntity> Create(TEntity entity, string createdBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var validationResult = unitOfWork.Repository<TContext, TEntity>().Insert(entity, createdBy);
                if (validationResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                }

                var saveResult = unitOfWork.Complete();
                if (saveResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(saveResult.ObjectValidationErrors);
                }

                return Result.Ok(entity);
            }
        }

        public virtual async Task<Result<TEntity>> CreateAsync(CancellationToken cancellationToken, TEntity entity, string createdBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(UnitOfWorkScopeOption.JoinExisting))
            {
                var validationResult = await unitOfWork.Repository<TContext, TEntity>().InsertAsync(cancellationToken, entity, createdBy);
                if (validationResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                }

                var saveResult = await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
                if (saveResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(saveResult.ObjectValidationErrors);
                }

                return Result.Ok(entity);
            }
        }
        #endregion

        #region Update
        public virtual Result Update(TEntity entity, string updatedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, updatedBy);
                if (validationResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                }

                return unitOfWork.Complete();
            }
        }

        public virtual async Task<Result> UpdateAsync(CancellationToken cancellationToken, TEntity entity, string updatedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(UnitOfWorkScopeOption.JoinExisting))
            {
                var validationResult = await unitOfWork.Repository<TContext, TEntity>().UpdateAsync(cancellationToken, entity, updatedBy);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

                return await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public virtual Result UpdateGraph(TEntity entity, string updatedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var validationResult = unitOfWork.Repository<TContext, TEntity>().UpdateGraph(entity, updatedBy);
                if (validationResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                }

                return unitOfWork.Complete();
            }
        }

        public virtual async Task<Result> UpdateGraphAsync(CancellationToken cancellationToken, TEntity entity, string updatedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(UnitOfWorkScopeOption.JoinExisting))
            {
                var validationResult = await unitOfWork.Repository<TContext, TEntity>().UpdateGraphAsync(cancellationToken, entity, updatedBy);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

               return await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion

        #region Delete
        public virtual Result Delete(object id, string deletedBy)
        {
            TEntity entity = GetById(id);
            return Delete(entity, deletedBy);
        }

        public virtual async Task<Result> DeleteAsync(CancellationToken cancellationToken, object id, string deletedBy)
        {
            TEntity entity = await GetByIdAsync(id, cancellationToken);
            return await DeleteAsync(cancellationToken, entity, deletedBy);
        }

        public virtual Result Delete(TEntity entity, string deletedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var validationResult = unitOfWork.Repository<TContext, TEntity>().Delete(entity, deletedBy);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

               return unitOfWork.Complete();
            }
        }

        public virtual async Task<Result> DeleteAsync(CancellationToken cancellationToken, TEntity entity, string deletedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(UnitOfWorkScopeOption.JoinExisting))
            {
                var validationResult = await unitOfWork.Repository<TContext, TEntity>().DeleteAsync(cancellationToken, entity, deletedBy);
                if (validationResult.IsFailure)
                {
                    return validationResult;
                }

                return await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
            }
        }
        #endregion

        #region TriggerActions
        public Result TriggerAction(object id, string action, string triggeredBy)
        {
            var actionEvents = new ActionEvents();

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var entity = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetById(id);

                if (entity is IEntityAggregateRoot)
                {
                    IDomainActionEvent actionEvent = actionEvents.CreateEntityActionEvent(action, null, entity, triggeredBy);
                    if (actionEvent != null)
                    {
                        ((IEntityAggregateRoot)entity).AddActionEvent(actionEvent);

                        var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, triggeredBy);
                        if (validationResult.IsFailure)
                        {
                            return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                        }

                       return unitOfWork.Complete();
                    }
                }
            }

            return Result.Ok();
        }

        public async Task<Result> TriggerActionAsync(object id, string action, string triggeredBy, CancellationToken cancellationToken)
        {
            var actionEvents = new ActionEvents();

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var entity = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(cancellationToken, id);
                if (entity is IEntityAggregateRoot)
                {
                    IDomainActionEvent actionEvent = actionEvents.CreateEntityActionEvent(action, null, entity, triggeredBy);
                    if (actionEvent != null)
                    {
                        ((IEntityAggregateRoot)entity).AddActionEvent(actionEvent);

                        var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, triggeredBy);
                        if (validationResult.IsFailure)
                        {
                            return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                        }

                      return await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            return Result.Ok();
        }
        #endregion
    }
}
