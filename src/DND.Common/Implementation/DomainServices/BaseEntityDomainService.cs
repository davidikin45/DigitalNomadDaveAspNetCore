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
                if (!(Exists(entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Update(entity, "");
                    unitOfWork.Complete();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6UpdateConcurrency(ex);
                }
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
                if (!(await ExistsAsync(cancellationToken, entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Update(entity, "");
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

                if(!(v1 == null && v2 == null))
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

        public virtual Result Delete(object id)
        {
            TEntity entity = GetById(id);
            return Delete(entity);
        }

        public virtual async Task<Result> DeleteAsync(object id, CancellationToken cancellationToken)
        {
            TEntity entity = await GetByIdAsync(id, cancellationToken);
            return await DeleteAsync(entity, cancellationToken);
        }

        public virtual Result Delete(TEntity entity)
        {

            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                if (!(Exists(entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Delete(entity);
                    unitOfWork.Complete();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return HandleEF6DeleteConcurrency(ex);
                }

            }

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken)
        {

            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                if (!(await ExistsAsync(cancellationToken, entity.Id)))
                {
                    return Result.ObjectDoesNotExist();
                }

                try
                {
                    unitOfWork.Repository<TContext, TEntity>().Delete(entity);
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

    }
}
