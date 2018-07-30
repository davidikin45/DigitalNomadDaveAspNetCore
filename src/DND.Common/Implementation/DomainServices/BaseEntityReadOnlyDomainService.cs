using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.DomainServices
{
    public abstract class BaseEntityReadOnlyDomainService<TContext, TEntity> : BaseDomainService, IBaseEntityReadOnlyDomainService<TEntity>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
    {

        public BaseEntityReadOnlyDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        public virtual void AddIncludes(List<Expression<Func<TEntity, Object>>> includes)
        {

        }

        public virtual bool IncludeAllCompositionRelationshipProperties => false;
        public virtual bool IncludeAllCompositionAndAggregationRelationshipProperties => false;

        protected virtual IQueryable<TEntity> GetQueryable(
          IEnumerable<TEntity> list,
          Expression<Func<TEntity, bool>> filter = null,
          Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> orderBy = null,
          int? skip = null,
          int? take = null)
        {
            //includeProperties = includeProperties ?? string.Empty;
            IQueryable<TEntity> query = list.AsQueryable();


            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy.Compile()(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query;
        }

        #region GetAll
        public virtual IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAllNoTracking(orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties);

                return entityList;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAllNoTrackingAsync(orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);

                return entityList;
            }
        }
        #endregion

        #region Search
        public virtual IEnumerable<TEntity> Search(
        string search = "",
        Expression<Func<TEntity, bool>> filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int? pageNo = null,
        int? pageSize = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().SearchNoTracking(search, filter, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties);

                return entityList;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().SearchNoTrackingAsync(search, filter, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);

                return entityList;
            }
        }

        public virtual int GetSearchCount(
    string search = "",
   Expression<Func<TEntity, bool>> filter = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetSearchCount(search, filter);
            }
        }

        public virtual async Task<int> GetSearchCountAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TEntity, bool>> filter = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetSearchCountAsync(search, filter).ConfigureAwait(false);
            }
        }
        #endregion

        #region Get
        public virtual IEnumerable<TEntity> Get(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         int? pageNo = null,
         int? pageSize = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetNoTracking(filter, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties,  includeProperties);

                return entityList;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetNoTrackingAsync(filter, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);

                return entityList;
            }
        }

        public virtual int GetCount(
    Expression<Func<TEntity, bool>> filter = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetCount(filter);
            }
        }

        public virtual async Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetCountAsync(filter).ConfigureAwait(false);
            }
        }
        #endregion

        #region GetOne
        public virtual TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetOne(filter, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties);
            }
        }

        public virtual async Task<TEntity> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetOneAsync(filter, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
            }
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
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetFirst(filter, orderBy, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties);
            }
        }

        public virtual async Task<TEntity> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetFirstAsync(filter, orderBy, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
            }
        }
        #endregion

        #region GetById
        public virtual TEntity GetById(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetById(id, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken), bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(id, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
            }
        }

        #endregion

        #region  GetByIdWithPagedCollectionProperty
        public virtual TEntity GetByIdWithPagedCollectionProperty(object id, string collectionProperty, int? pageNo = null, int? pageSize = null, object collectionItemId = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionProperty(id, collectionProperty, pageNo, pageSize, collectionItemId);
            }
        }

        public virtual async Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, object id, string collectionProperty, int? pageNo = null, int? pageSize = null, object collectionItemId = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionPropertyAsync(id, collectionProperty, pageNo, pageSize, collectionItemId).ConfigureAwait(false);
            }
        }

        public virtual int GetByIdWithPagedCollectionPropertyCount(object id, string collectionProperty)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionPropertyCount(id, collectionProperty);
            }
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, object id, string collectionProperty)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionPropertyCountAsync(id, collectionProperty).ConfigureAwait(false);
            }
        }
        #endregion

        #region GetByIds
        public virtual IEnumerable<TEntity> GetByIds(IEnumerable<object> ids,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdsNoTracking(ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(CancellationToken cancellationToken,
            IEnumerable<object> ids,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var includes = includeProperties != null ? includeProperties.ToList() : new List<Expression<Func<TEntity, Object>>>();
            AddIncludes(includes);
            includeProperties = includes.ToArray();

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdsNoTrackingAsync(ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
            }
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().ExistsNoTracking(filter);
            }
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null
            )
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().ExistsNoTrackingAsync(filter).ConfigureAwait(false);
            }
        }

        public virtual bool Exists(object id)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().ExistsById(id);
            }
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id
            )
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().ExistsByIdAsync(id).ConfigureAwait(false);
            }
        }
        #endregion
    }
}
