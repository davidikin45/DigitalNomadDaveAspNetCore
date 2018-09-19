using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.DomainServices
{
    public abstract class DomainServiceEntityReadOnlyBase<TContext, TEntity> : DomainServiceBase, IDomainServiceEntityReadOnly<TEntity>
          where TContext : DbContext
          where TEntity : class
    {

        public DomainServiceEntityReadOnlyBase(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAllNoTrackingAsync(cancellationToken, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);

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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().SearchNoTrackingAsync(cancellationToken, search, filter, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);

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
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetSearchCountAsync(cancellationToken, search, filter).ConfigureAwait(false);
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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetNoTrackingAsync(cancellationToken, filter, orderBy, pageNo * pageSize, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);

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
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetCountAsync(cancellationToken, filter).ConfigureAwait(false);
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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetOneAsync(cancellationToken, filter, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetFirstAsync(cancellationToken, filter, orderBy, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(cancellationToken, id, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
            }
        }

        #endregion

        #region  GetByIdWithPagedCollectionProperty
        public virtual TEntity GetByIdWithPagedCollectionProperty(object id, 
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionProperty(id, collectionExpression, search, orderBy, ascending, pageNo * pageSize, pageSize);
            }
        }

        public virtual async Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionPropertyAsync(cancellationToken, id, collectionExpression, search, orderBy, ascending, pageNo * pageSize, pageSize).ConfigureAwait(false);
            }
        }

        public virtual int GetByIdWithPagedCollectionPropertyCount(object id, string collectionExpression, string search = "")
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionPropertyCount(id, collectionExpression, search);
            }
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, object id, string collectionExpression, string search = "")
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdWithPagedCollectionPropertyCountAsync(cancellationToken, id, collectionExpression, search).ConfigureAwait(false);
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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdsNoTrackingAsync(cancellationToken, ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includeProperties).ConfigureAwait(false);
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
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().ExistsNoTrackingAsync(cancellationToken, filter).ConfigureAwait(false);
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
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().ExistsByIdAsync(cancellationToken, id).ConfigureAwait(false);
            }
        }
        #endregion

        #region Validate
        public Result Validate(TEntity entity, ValidationMode mode)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().Validate(entity, mode);
            }
        }

        public async Task<Result> ValidateAsync(CancellationToken cancellationToken, TEntity entity, ValidationMode mode)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().ValidateAsync(cancellationToken, entity, mode);
            }
        }
        #endregion
    }
}
