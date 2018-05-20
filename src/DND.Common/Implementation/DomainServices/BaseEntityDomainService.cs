using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Extensions;
using DND.Common.Enums;

namespace DND.Common.Implementation.DomainServices
{
    public abstract class BaseEntityDomainService<TContext, TEntity> : BaseEntityReadOnlyDomainService<TContext, TEntity>, IBaseEntityDomainService<TEntity>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, IBaseEntityConcurrencyAware, new()
    {

        public BaseEntityDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        public virtual Result<TEntity> Create(TEntity entity, string createdBy)
        {
            var validationResult = Validate(entity, ValidationMode.Create);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
            }

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Create(entity, createdBy);
                unitOfWork.Complete();

                return Result.Ok(entity);
            }
        }

        public virtual async Task<Result<TEntity>> CreateAsync(TEntity entity, string createdBy, CancellationToken cancellationToken)
        {
            var validationResult = await ValidateAsync(entity, ValidationMode.Create);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail<TEntity>(validationResult.ObjectValidationErrors);
            }

            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Create(entity, createdBy);
                await unitOfWork.CompleteAsync(cancellationToken);

                return Result.Ok(entity);
            }
        }

        public virtual Result Update(TEntity entity, string updatedBy)
        {
            var validationResult = Validate(entity, ValidationMode.Update);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                if (!(Exists(entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Update(entity, updatedBy);
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
            var validationResult = await ValidateAsync(entity, ValidationMode.Update);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                if (!(await ExistsAsync(cancellationToken, entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Update(entity, updatedBy);
                    await unitOfWork.CompleteAsync(cancellationToken);
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
            var validationResult = Validate(entity, ValidationMode.Delete);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                if (!(Exists(entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Delete(entity, deletedBy);
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
            var validationResult = await ValidateAsync(entity, ValidationMode.Delete);

            if (validationResult.IsFailure)
            {
                return validationResult;
            }

            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                if (!(await ExistsAsync(cancellationToken, entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Delete(entity, deletedBy);
                    await unitOfWork.CompleteAsync(cancellationToken);
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
            var task = ValidateAsync(entity, mode);
            task.Wait();
            return task.Result;
        }

        public async Task<Result> ValidateAsync(TEntity entity, ValidationMode mode)
        {
            if (mode != ValidationMode.Delete)
            {
                var objectValidationErrors = entity.Validate().ToList();
                if (objectValidationErrors.Any())
                {
                    return Result.ObjectValidationFail(objectValidationErrors);
                }
            }

            if (mode == ValidationMode.Create || mode == ValidationMode.Update || mode == ValidationMode.Delete)
            {
                var dbDependantValidationErrors = await DbDependantValidateAsync(entity, mode).ConfigureAwait(false);
                if (dbDependantValidationErrors.Any())
                {
                    return Result.ObjectValidationFail(dbDependantValidationErrors);
                }
            }

            return Result.Ok();
        }

        public abstract Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(TEntity entity, ValidationMode mode);
    }
}
