using DND.Common.Data.Helpers;
using DND.Common.Infrastructure.Extensions;
using DND.Common.Infrastructure.Helpers;
using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Data.Repository.GenericEF
{
    public class GenericEFRepository<TEntity> : GenericEFReadOnlyRepository<TEntity>, IGenericEFRepository<TEntity>
   where TEntity : class
    {
        public GenericEFRepository(DbContext context)
            : base(context)
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

        public async virtual Task<Result> InsertOrUpdateAsync(CancellationToken cancellationToken, TEntity entity, string createdModifiedBy = null)
        {
            var existingEntity = await _context.FindEntityAsync<TEntity>(entity, cancellationToken);
            if (existingEntity != null)
            {
                return await UpdateAsync(cancellationToken, entity, createdModifiedBy);
            }
            else
            {
                return await InsertAsync(cancellationToken, entity, createdModifiedBy);
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

            var autidableEntity = entity as IEntityAuditable;
            if(autidableEntity != null)
            {
                autidableEntity.DateCreated = DateTime.UtcNow;
                autidableEntity.UserCreated = createdBy;
            }

            _context.AddEntity(entity);

            return Result.Ok(entity);
        }

        public async virtual Task<Result> InsertAsync(CancellationToken cancellationToken, TEntity entity, string createdBy = null)
        {
            var validationResult = await ValidateAsync(cancellationToken, entity, ValidationMode.Insert).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            var autidableEntity = entity as IEntityAuditable;
            if (autidableEntity != null)
            {
                autidableEntity.DateCreated = DateTime.UtcNow;
                autidableEntity.UserCreated = createdBy;
            }

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
                var auditableEntity = entity as IEntityAuditable;
                if (auditableEntity != null)
                {
                    auditableEntity.DateModified = DateTime.UtcNow;
                    auditableEntity.UserModified = modifiedBy;
                }
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

        public async virtual Task<Result> UpdateAsync(CancellationToken cancellationToken, TEntity entity, string modifiedBy = null)
        {
            if (!(await ExistsAsync(cancellationToken, entity).ConfigureAwait(false)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = await ValidateAsync(cancellationToken, entity, ValidationMode.Update).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            if (!_context.IsEntityStateAdded(entity))
            {
                var auditableEntity = entity as IEntityAuditable;
                if (auditableEntity != null)
                {
                    auditableEntity.DateModified = DateTime.UtcNow;
                    auditableEntity.UserModified = modifiedBy;
                }
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

            var modifiedDateTime = DateTime.UtcNow;
            var auditableEntity = entity as IEntityAuditable;
            if (auditableEntity != null)
            {
                auditableEntity.DateModified = modifiedDateTime;
                auditableEntity.UserModified = modifiedBy;
            }

            SetCompositionPropertyForeignKeysAndAudit(entity, modifiedBy, modifiedDateTime);

            return Result.Ok(_context.UpdateGraph(entity));
        }

        public async virtual Task<Result> UpdateGraphAsync(CancellationToken cancellationToken, TEntity entity, string modifiedBy = null)
        {
            if (!(await ExistsNoTrackingAsync(cancellationToken, entity).ConfigureAwait(false)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = await ValidateAsync(cancellationToken, entity, ValidationMode.Update).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            var modifiedDateTime = DateTime.UtcNow;
            var auditableEntity = entity as IEntityAuditable;
            if (auditableEntity != null)
            {
                auditableEntity.DateModified = DateTime.UtcNow;
                auditableEntity.UserModified = modifiedBy;
            }

            SetCompositionPropertyForeignKeysAndAudit(entity, modifiedBy, modifiedDateTime);

            return Result.Ok(_context.UpdateGraph(entity));
        }

        private void SetCompositionPropertyForeignKeysAndAudit(Object entity, string modifiedBy, DateTime dateModified)
        {
            if (entity.HasProperty(nameof(IEntity.Id)))
            {
                var id = entity.GetPropValue(nameof(IEntity.Id));
                var idPropertyName = entity.GetType().Name + nameof(IEntity.Id);
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

                                var auditable = item as IEntityAuditable;
                                if (auditable != null)
                                {
                                    bool containsId = false;
                                    string itemId = "";
                                    if (item.HasProperty(nameof(IEntity.Id)))
                                    {
                                        containsId = true;
                                        itemId = item.GetPropValue(nameof(IEntity.Id)).ToString();
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
            TEntity entity = GetById(id); // For concurrency purposes need to get latest version

            return Delete(entity, deletedBy);
        }

        public async virtual Task<Result> DeleteAsync(CancellationToken cancellationToken, object id, string deletedBy = null)
        {
            TEntity entity = await GetByIdAsync(cancellationToken, id); // For concurrency purposes need to get latest version

            return await DeleteAsync(cancellationToken, entity, deletedBy);
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

            var auditableEntity = entity as IEntityAuditable;
            if (auditableEntity != null)
            {
                auditableEntity.DateDeleted = DateTime.UtcNow;
                auditableEntity.UserDeleted = deletedBy;
            }

            _context.RemoveEntity(entity);

            return Result.Ok();
        }

        public async virtual Task<Result> DeleteAsync(CancellationToken cancellationToken, TEntity entity, string deletedBy = null)
        {
            if (!(await ExistsNoTrackingAsync(cancellationToken, entity).ConfigureAwait(false)))
            {
                return Result.ObjectDoesNotExist();
            }

            var validationResult = await ValidateAsync(cancellationToken, entity, ValidationMode.Delete).ConfigureAwait(false);

            if (validationResult.IsFailure)
            {
                return Result.ObjectValidationFail(validationResult.ObjectValidationErrors);
            }

            if (_context.IsEntityStateDetached(entity))
            {
                _context.AttachEntity(entity);
            }

            var auditableEntity = entity as IEntityAuditable;
            if (auditableEntity != null)
            {
                auditableEntity.DateDeleted = DateTime.UtcNow;
                auditableEntity.UserDeleted = deletedBy;
            }

            _context.RemoveEntity(entity);

            return Result.Ok();
        }
        #endregion
    }
}
