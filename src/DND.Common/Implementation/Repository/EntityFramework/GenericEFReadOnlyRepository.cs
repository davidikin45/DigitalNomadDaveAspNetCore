using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Repository;
using DND.Common.Interfaces.Data;

using System.Threading.Tasks;
using System.Data.Entity;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using DND.Common.Implementation.Validation;
using DND.Common.Enums;
using DND.Common.Interfaces.UnitOfWork;
using DND.Common.Helpers;
using DND.Common.Extensions;
using System.Data.Entity.Spatial;
using DND.Common.Implementation.Data;

namespace DND.Common.Implementation.Repository.EntityFramework
{
    public class GenericEFReadOnlyRepository<TEntity> : IGenericEFReadOnlyRepository<TEntity>
    where TEntity : class, IBaseEntity
    {
        protected readonly IBaseUnitOfWorkScope _uow;
        protected readonly IBaseDbContext _context;
        protected readonly CancellationToken _cancellationToken;

        //AsNoTracking() causes EF to bypass cache for reads and writes - Ideal for Web Applications and short lived contexts

        public GenericEFReadOnlyRepository(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken))
        {
            this._uow = uow;
            this._context = context;
            this._cancellationToken = cancellationToken;
        }

        protected virtual IQueryable<TEntity> GetQueryable(
            bool tracking,
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            //includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = _context.Queryable<TEntity>();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = LamdaHelper.CreateSearchQuery(query, search);
            }

            if (includeAllCompositionAndAggregationRelationshipProperties)
            {
                var includesList = RelationshipHelper.GetAllCompositionAndAggregationRelationshipPropertyIncludes(false, typeof(TEntity));

                foreach (var include in includesList)
                {
                    query = query.Include(include);
                }
            }
            else
            {
                if (includeAllCompositionRelationshipProperties)
                {
                    //For Aggregate Roots
                    var includesList = RelationshipHelper.GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, typeof(TEntity));

                    foreach (var include in includesList)
                    {
                        query = query.Include(include);
                    }
                }

                if (includeProperties != null)
                {
                    foreach (var includeExpression in includeProperties)
                    {
                        query = query.Include(includeExpression);
                    }
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                //https://visualstudiomagazine.com/articles/2016/12/06/skip-take-entity-framework-lambda.aspx
                Expression<Func<int>> countAccessor = () => skip.Value;
                query = query.Skip(countAccessor);
            }

            if (take.HasValue)
            {
                //https://visualstudiomagazine.com/articles/2016/12/06/skip-take-entity-framework-lambda.aspx
                Expression<Func<int>> countAccessor = () => take.Value;
                query = query.Take(countAccessor);
            }

            DebugSQL(query);

            return query;
        }

        private void DebugSQL(IQueryable<TEntity> query)
        {
            var sql = query.ToString();
        }

        #region SQLQuery
        public virtual IEnumerable<TEntity> SQLQuery(string query, params object[] paramaters)
        {
            return _context.SQLQueryNoTracking<TEntity>(query, paramaters);
        }

        public async virtual Task<IEnumerable<TEntity>> SQLQueryAsync(string query, params object[] paramaters)
        {
            return await _context.SQLQueryNoTrackingAsync<TEntity>(query, paramaters).ConfigureAwait(false);
        }
        #endregion

        #region GetAll
        public virtual IEnumerable<TEntity> GetAll(
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         int? skip = null,
         int? take = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, null, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            // string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> GetAllNoTracking(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int? skip = null,
        int? take = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, null, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllNoTrackingAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            // string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Get
        public virtual IEnumerable<TEntity> Get(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> GetNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, filter).Count();
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, filter).CountAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Search
        public virtual IEnumerable<TEntity> Search(
          string search = "",
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, search, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, search, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> SearchNoTracking(
          string search = "",
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, search, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> SearchNoTrackingAsync(
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, search, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, search, filter).Count();
        }

        public virtual async Task<int> GetSearchCountAsync(string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, search, filter).CountAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetOne
        public virtual TEntity GetOne(
         Expression<Func<TEntity, bool>> filter = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync(
          Expression<Func<TEntity, bool>> filter = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetOneNoTracking(
         Expression<Func<TEntity, bool>> filter = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneNoTrackingAsync(
          Expression<Func<TEntity, bool>> filter = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetFirst
        public virtual TEntity GetFirst(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, filter, orderBy, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, orderBy, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).FirstOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetFirstNoTracking(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, orderBy, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, orderBy, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).FirstOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetById
        public virtual TEntity GetById(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            //return _context.FindEntityById<TEntity>(id);
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);

            return GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
        }

        public virtual TEntity GetByIdNoTracking(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);
            return GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
        }

        public async virtual Task<TEntity> GetByIdAsync(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);
            return await GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public async virtual Task<TEntity> GetByIdNoTrackingAsync(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);
            return await GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetByIdWithPagedCollectionProperty
        public virtual TEntity GetByIdWithPagedCollectionProperty(object id, 
            string collectionProperty,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null, 
            int? take = null, 
            object collectionItemId = null)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.LoadCollectionProperty(entity, collectionProperty, search, orderBy, ascending, skip, take, collectionItemId);
            }
            return entity;
        }

        public async virtual Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(object id, 
            string collectionProperty,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null, 
            int? take = null, 
            object collectionItemId = null)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                await _context.LoadCollectionPropertyAsync(entity, collectionProperty, search, orderBy, ascending, skip, take, collectionItemId);
            }
            return entity;
        }

        public virtual int GetByIdWithPagedCollectionPropertyCount(object id, string collectionProperty, string search = "")
        {
            var entity = GetById(id);
            if (entity != null)
            {
                return _context.CollectionPropertyCount(entity, collectionProperty, search);
            }
            return 0;
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(object id, string collectionProperty, string search = "")
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                return await _context.CollectionPropertyCountAsync(entity, collectionProperty, search, _cancellationToken);
            }
            return 0;
        }
        #endregion

        #region GetByIds
        public virtual IEnumerable<TEntity> GetByIds(IEnumerable<object> ids,
      bool includeAllCompositionRelationshipProperties = false,
      bool includeAllCompositionAndAggregationRelationshipProperties = false,
      params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return GetQueryable(true, null, filter, null, null).ToList();
        }

        public virtual IEnumerable<TEntity> GetByIdsNoTracking(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return GetQueryable(false, null, filter, null, null).ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return await GetQueryable(false, null, filter, null, null).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public async virtual Task<IEnumerable<TEntity>> GetByIdsNoTrackingAsync(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return await GetQueryable(false, null, filter, null, null).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(true, null, filter).ToList().Any();
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return (await GetQueryable(true, null, filter).ToListAsync(_cancellationToken).ConfigureAwait(false)).Any();
        }

        public virtual bool ExistsNoTracking(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, filter).Any();
        }

        public virtual async Task<bool> ExistsNoTrackingAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, filter).AnyAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual bool Exists(TEntity entity)
        {
            return _context.EntityExists(entity);
        }

        public virtual async Task<bool> ExistsAsync(TEntity entity)
        {
            return await _context.EntityExistsAsync(entity, _cancellationToken);
        }

        public virtual bool ExistsNoTracking(TEntity entity)
        {
            return _context.EntityExistsNoTracking(entity);
        }

        public virtual async Task<bool> ExistsNoTrackingAsync(TEntity entity)
        {
            return await _context.EntityExistsNoTrackingAsync(entity, _cancellationToken);
        }

        public virtual bool ExistsById(object id)
        {
            return _context.EntityExistsById<TEntity>(id);
        }

        public virtual async Task<bool> ExistsByIdAsync(object id)
        {
            return await _context.EntityExistsByIdAsync<TEntity>(id, _cancellationToken).ConfigureAwait(false);
        }

        public virtual bool ExistsByIdNoTracking(object id)
        {
            return _context.EntityExistsByIdNoTracking<TEntity>(id);
        }

        public virtual async Task<bool> ExistsByIdNoTrackingAsync(object id)
        {
            return await _context.EntityExistsByIdNoTrackingAsync<TEntity>(id, _cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Validate

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

            if (mode == ValidationMode.Insert || mode == ValidationMode.Update || mode == ValidationMode.Delete)
            {
                var dbDependantValidationErrors = await entity.ValidateWithDbConnectionAsync(_uow, mode).ConfigureAwait(false);
                if (dbDependantValidationErrors.Any())
                {
                    return Result.ObjectValidationFail(dbDependantValidationErrors);
                }
            }

            return Result.Ok();
        }
        #endregion
    }
}
