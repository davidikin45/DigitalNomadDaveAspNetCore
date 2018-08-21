using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Helpers;
using DND.Common.Implementation.Data;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.UnitOfWork;
using RefactorThis.GraphDiff;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Repository.EntityFramework

// Setting state manually is important in case of detached entities (entities loaded without change tracking or created outside of the current context).
{
    public class GenericEFRepository<TEntity> : GenericEFReadOnlyRepository<TEntity>, IGenericEFRepository<TEntity>
     where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
    {
        public GenericEFRepository(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken))
            : base(context, uow, cancellationToken)
        {
        }

        #region InsertOrUpdate
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
        #endregion

        #region Insert
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
        #endregion

        #region Update
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

        public virtual Result UpdateGraph(TEntity entity, string modifiedBy = null)
        {
            if (!(ExistsNoTracking(entity)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = Validate(entity, ValidationMode.Update);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            entity.DateModified = DateTime.UtcNow;
            entity.UserModified = modifiedBy;

            SetCompositionPropertyForeignKeysAndAudit(entity, entity.UserModified, entity.DateModified.Value);

            Expression<Func<IUpdateConfiguration<TEntity>, object>> graphDiffConfiguration = (Expression<Func<IUpdateConfiguration<TEntity>, object>>)LamdaHelper.GraphDiffConfiguration(typeof(TEntity));

            return Result.Ok(_context.UpdateGraph(entity, graphDiffConfiguration));
        }

        public async virtual Task<Result> UpdateGraphAsync(TEntity entity, string modifiedBy = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!(await ExistsNoTrackingAsync(entity).ConfigureAwait(false)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = await ValidateAsync(entity, ValidationMode.Update).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            entity.DateModified = DateTime.UtcNow;
            entity.UserModified = modifiedBy;

            SetCompositionPropertyForeignKeysAndAudit(entity, entity.UserModified, entity.DateModified.Value);

            Expression<Func<IUpdateConfiguration<TEntity>, object>> graphDiffConfiguration = (Expression<Func<IUpdateConfiguration<TEntity>, object>>)LamdaHelper.GraphDiffConfiguration(typeof(TEntity));

            return Result.Ok(_context.UpdateGraph(entity, graphDiffConfiguration));
        }

        private void SetCompositionPropertyForeignKeysAndAudit(Object entity, string modifiedBy, DateTime dateModified)
        {
            if (entity.HasProperty(nameof(IBaseEntity.Id)))
            {
                var id = entity.GetPropValue(nameof(IBaseEntity.Id));
                var idPropertyName = entity.GetType().Name + nameof(IBaseEntity.Id);
                var compositionProperties = RelationshipHelper.GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, entity.GetType()).Where(p => !p.Contains("."));
                foreach (var compositionProperty in compositionProperties)
                {
                    var prop = typeof(TEntity).GetProperty(compositionProperty);
                    var collection = prop.GetValue(entity) as IEnumerable;
                    if (collection != null)
                    {
                        foreach (var item in collection)
                        {
                            if (item.HasProperty(idPropertyName))
                            {
                                item.SetPropValue(idPropertyName, id);

                                var auditable = item as IBaseEntityAuditable;
                                if (auditable != null)
                                {
                                    bool containsId = false;
                                    string itemId = "";
                                    if (item.HasProperty(nameof(IBaseEntity.Id)))
                                    {
                                        containsId = true;
                                        itemId = item.GetPropValue(nameof(IBaseEntity.Id)).ToString();
                                    }

                                    if (!string.IsNullOrWhiteSpace(auditable.UserCreated) || (containsId && (string.IsNullOrWhiteSpace(itemId) || itemId == "0" || itemId == Guid.Empty.ToString())))
                                    {
                                        auditable.UserCreated = modifiedBy;
                                        auditable.DateCreated = dateModified;
                                    }
                                    else
                                    {
                                        auditable.UserModified = modifiedBy;
                                        auditable.DateModified = dateModified;
                                    }
                                }
                                SetCompositionPropertyForeignKeysAndAudit(item, modifiedBy, dateModified);
                            }
                        }
                    }
                }
            }
        }
        #endregion

            #region Delete
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
            if (!(ExistsNoTracking(entity)))
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
            if (!(await ExistsNoTrackingAsync(entity).ConfigureAwait(false)))
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
        #endregion
    }

}
