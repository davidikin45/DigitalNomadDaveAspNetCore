using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.Data;

using System.Data.Entity;
using RefactorThis.GraphDiff;
using System.Linq.Expressions;
using System.Threading;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Enums;
using DND.Common.Implementation.Validation;

namespace DND.Common.Implementation.Repository.EntityFramework

// Setting state manually is important in case of detached entities (entities loaded without change tracking or created outside of the current context).
{
    public class GenericEFRepository<TEntity> : GenericEFReadOnlyRepository<TEntity>, IBaseRepository<TEntity>
     where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
    {
        public GenericEFRepository(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken))
            : base(context, uow, cancellationToken)
        {
        }

        public virtual Result InsertOrUpdate(TEntity entity, string createdModifiedBy = null)
        {
            var existingEntity = _context.FindEntity<TEntity>(entity);
            if (existingEntity != null)
            {
                return Update(entity, createdModifiedBy);
            }
            else
            {
                return Insert(entity, createdModifiedBy);
            }
        }

        public async virtual Task<Result> InsertOrUpdateAsync(TEntity entity, string createdModifiedBy = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var existingEntity = await _context.FindEntityAsync<TEntity>(entity, cancellationToken);
            if (existingEntity != null)
            {
                return await UpdateAsync(entity, createdModifiedBy, cancellationToken);
            }
            else
            {
                return await InsertAsync(entity, createdModifiedBy, cancellationToken);
            }
        }

        public virtual Result Insert(TEntity entity, string createdBy = null)
        {
            var validationResult = Validate(entity, ValidationMode.Insert);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            entity.DateCreated = DateTime.UtcNow;
            entity.UserCreated = createdBy;
            _context.AddEntity(entity);

            return Result.Ok(entity);
        }

        public async virtual Task<Result> InsertAsync(TEntity entity, string createdBy = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var validationResult = await ValidateAsync(entity, ValidationMode.Insert).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            entity.DateCreated = DateTime.UtcNow;
            entity.UserCreated = createdBy;
            _context.AddEntity(entity);

            return Result.Ok(entity);
        }

        public virtual Result Update(TEntity entity, string modifiedBy = null)
        {
            //This will attach the Entity and ensure it exists
            if (!(Exists(entity)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = Validate(entity, ValidationMode.Update);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            if (!_context.IsEntityStateAdded(entity))
            {
                entity.DateModified = DateTime.UtcNow;
                entity.UserModified = modifiedBy;
            }

            if (_context.IsEntityStateDetached(entity))
            {
                var existingEntity = _context.FindEntity(entity);
                _context.UpdateEntity(existingEntity, entity);
            }
            else
            {
                if (!_context.IsEntityStateAdded(entity))
                {
                    _context.TriggerTrackChanges(entity);
                }
            }

            return Result.Ok();

            //When you change the state to Modified all the properties of the entity will be marked as modified and all the property values will be sent to the database when SaveChanges is called.
            //Note that if the entity being attached has references to other entities that are not yet tracked, then these new entities will attached to the context in the Unchanged state—they will not automatically be made Modified.If you have multiple entities that need to be marked Modified you should set the state for each of these entities individually.          
        }

        public async virtual Task<Result> UpdateAsync(TEntity entity, string modifiedBy = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!(await ExistsAsync(entity).ConfigureAwait(false)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = await ValidateAsync(entity, ValidationMode.Update).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            if (!_context.IsEntityStateAdded(entity))
            {
                entity.DateModified = DateTime.UtcNow;
                entity.UserModified = modifiedBy;
            }

            if (_context.IsEntityStateDetached(entity))
            {
                var existingEntity = _context.FindEntity(entity);
                _context.UpdateEntity(existingEntity, entity);
            }
            else
            {
                if (!_context.IsEntityStateAdded(entity))
                {
                    _context.TriggerTrackChanges(entity);
                }
            }

            return Result.Ok();

            //When you change the state to Modified all the properties of the entity will be marked as modified and all the property values will be sent to the database when SaveChanges is called.
            //Note that if the entity being attached has references to other entities that are not yet tracked, then these new entities will attached to the context in the Unchanged state—they will not automatically be made Modified.If you have multiple entities that need to be marked Modified you should set the state for each of these entities individually.          
        }


        public virtual TEntity UpdateGraph(TEntity entity, Expression<Func<IUpdateConfiguration<TEntity>, object>> mapping = null, string modifiedBy = null)
        {
            entity.DateModified = DateTime.UtcNow;
            entity.UserModified = modifiedBy;
            return _context.UpdateGraph(entity, mapping);
        }

        public virtual Result Delete(object id, string deletedBy = null)
        {
            //TEntity entity = _context.FindEntityLocal<TEntity>(id);
            //if(entity == null)
            //{
            //    entity = new TEntity() { Id = id };
            //}
            TEntity entity = _context.FindEntityById<TEntity>(id); // For concurrency purposes need to get latest version

            return Delete(entity, deletedBy);
        }

        public async virtual Task<Result> DeleteAsync(object id, string deletedBy = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            //TEntity entity = _context.FindEntityLocal<TEntity>(id);
            //if(entity == null)
            //{
            //    entity = new TEntity() { Id = id };
            //}
            TEntity entity = await GetByIdAsync(id); // For concurrency purposes need to get latest version

            return await DeleteAsync(entity, deletedBy, cancellationToken);
        }

        public virtual Result Delete(TEntity entity, string deletedBy = null)
        {
            if (!(Exists(entity)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = Validate(entity, ValidationMode.Delete);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            if (_context.IsEntityStateDetached(entity))
            {
                _context.AttachEntity(entity);
            }
            entity.DateDeleted = DateTime.UtcNow;
            entity.UserDeleted = deletedBy;
            _context.RemoveEntity(entity);

            return Result.Ok();
        }

        public async virtual Task<Result> DeleteAsync(TEntity entity, string deletedBy = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!(await ExistsAsync(entity).ConfigureAwait(false)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = await ValidateAsync(entity, ValidationMode.Delete).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            if (_context.IsEntityStateDetached(entity))
            {
                _context.AttachEntity(entity);
            }
            entity.DateDeleted = DateTime.UtcNow;
            entity.UserDeleted = deletedBy;
            _context.RemoveEntity(entity);

            return Result.Ok();
        }
    }

}
