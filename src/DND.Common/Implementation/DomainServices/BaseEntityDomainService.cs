using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Extensions;
using DND.Common.Enums;
using DND.Common.DomainEvents.ActionEvent;
using DND.Common.DomainEvents;

namespace DND.Common.Implementation.DomainServices
{
    //Todo - Move validation down a level into repository
    public abstract class BaseEntityDomainService<TContext, TEntity> : BaseEntityReadOnlyDomainService<TContext, TEntity>, IBaseEntityDomainService<TEntity>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, IBaseEntityConcurrencyAware, new()
    {

        public BaseEntityDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        public virtual Result<TEntity> Create(TEntity entity, string createdBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var validationResult = unitOfWork.Repository<TContext, TEntity>().Insert(entity, createdBy);
                if (validationResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                }

                unitOfWork.Complete();

                return Result.Ok(entity);
            }
        }

        public virtual async Task<Result<TEntity>> CreateAsync(TEntity entity, string createdBy, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var validationResult = unitOfWork.Repository<TContext, TEntity>().Insert(entity, createdBy);
                if (validationResult.IsFailure)
                {
                    return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                }

                await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);

                return Result.Ok(entity);
            }
        }

        public virtual Result Update(TEntity entity, string updatedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                try
                {
                    var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, updatedBy);
                    if (validationResult.IsFailure)
                    {
                        return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                    }

                    unitOfWork.Complete();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6UpdateConcurrency(ex);
                }
            }

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateAsync(TEntity entity, string updatedBy, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                try
                {
                    var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, updatedBy);
                    if (validationResult.IsFailure)
                    {
                        return validationResult;
                    }

                    await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6UpdateConcurrency(ex);
                }
            }

            return Result.Ok();
        }

        private Result HandleEF6UpdateConcurrency(DbUpdateConcurrencyException ex)
        {
            var errors = new List<ValidationResult>();

            var entry = ex.Entries.Single();
            var clientValues = (TEntity)entry.Entity;
            var databaseEntry = entry.GetDatabaseValues();
            if (databaseEntry == null)
            {
                return Result.ConcurrencyConflict("Unable to save changes. Object was deleted by another user.");
            }

            var databaseValues = (TEntity)databaseEntry.ToObject();

            foreach (var prop in databaseValues.GetProperties())
            {
                var v1 = clientValues.GetPropValue(prop.Name);
                var v2 = databaseValues.GetPropValue(prop.Name);

                if (!(v1 == null && v2 == null))
                {
                    if (((v1 == null && v2 != null) || (v2 == null && v1 != null) || !v1.Equals(v2)))
                    {
                        var v2String = v2 == null ? "" : v2.ToString();

                        errors.Add(new ValidationResult("Current value: " + v2String, new string[] { prop.Name }));
                    }
                }
            }

            errors.Add(new ValidationResult("The record you attempted to edit "
                + "was modified by another user after you got the original value. The "
                + "edit operation was canceled and the current values in the database "
                + "have been returned. If you still want to edit this record, save "
                + "again."));

            return Result.ConcurrencyConflict(errors, databaseValues.RowVersion);
        }

        public virtual Result Delete(object id, string deletedBy)
        {
            TEntity entity = GetById(id);
            return Delete(entity, deletedBy);
        }

        public virtual async Task<Result> DeleteAsync(object id, string deletedBy, CancellationToken cancellationToken)
        {
            TEntity entity = await GetByIdAsync(id, cancellationToken);
            return await DeleteAsync(entity, deletedBy, cancellationToken);
        }

        public virtual Result Delete(TEntity entity, string deletedBy)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                try
                {
                    var validationResult = unitOfWork.Repository<TContext, TEntity>().Delete(entity, deletedBy);
                    if (validationResult.IsFailure)
                    {
                        return validationResult;
                    }

                    unitOfWork.Complete();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6DeleteConcurrency(ex);
                }

            }

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(TEntity entity, string deletedBy, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                try
                {
                    var validationResult = unitOfWork.Repository<TContext, TEntity>().Delete(entity, deletedBy);
                    if (validationResult.IsFailure)
                    {
                        return validationResult;
                    }

                    await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6DeleteConcurrency(ex);
                }
            }


            return Result.Ok();
        }

        private Result HandleEF6DeleteConcurrency(DbUpdateConcurrencyException ex)
        {
            var errors = new List<ValidationResult>();

            var entry = ex.Entries.Single();
            var clientValues = (TEntity)entry.Entity;
            var databaseEntry = entry.GetDatabaseValues();
            if (databaseEntry == null)
            {
                return Result.ConcurrencyConflict("Unable to save changes. Object was deleted by another user.");
            }

            var databaseValues = (TEntity)databaseEntry.ToObject();

            errors.Add(new ValidationResult("The record you attempted to delete "
            + "was modified by another user after you got the original values. "
            + "The delete operation was canceled and the current values in the "
            + "database have been returned. If you still want to delete this "
            + "record, Delete again."));

            return Result.ConcurrencyConflict(errors, databaseValues.RowVersion);
        }

        public Result Validate(TEntity entity, ValidationMode mode)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting))
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().Validate(entity, mode);
            }
        }

        public async Task<Result> ValidateAsync(TEntity entity, ValidationMode mode, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().ValidateAsync(entity, mode);
            }
        }

        public Result TriggerAction(object id, string action, dynamic args, string triggeredBy)
        {
            var actionEvents = new ActionEvents();

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                try
                {
                    var entity = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetById(id);

                    IDomainActionEvent actionEvent = actionEvents.CreateEntityActionEvent(action, args, entity, triggeredBy);
                    if(actionEvent != null)
                    {
                        entity.AddActionEvent(actionEvent);

                        var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, triggeredBy);
                        if (validationResult.IsFailure)
                        {
                            return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                        }

                        unitOfWork.Complete();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6UpdateConcurrency(ex);
                }
            }

            return Result.Ok();
        }

        public async Task<Result> TriggerActionAsync(object id, string action, dynamic args, string triggeredBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var actionEvents = new ActionEvents();

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                try
                {
                    var entity = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(id);

                    IDomainActionEvent actionEvent = actionEvents.CreateEntityActionEvent(action, args, entity, triggeredBy);
                    if (actionEvent != null)
                    {
                        entity.AddActionEvent(actionEvent);

                        var validationResult = unitOfWork.Repository<TContext, TEntity>().Update(entity, triggeredBy);
                        if (validationResult.IsFailure)
                        {
                            return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
                        }

                        await unitOfWork.CompleteAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6UpdateConcurrency(ex);
                }
            }

            return Result.Ok();
        }
    }
}
