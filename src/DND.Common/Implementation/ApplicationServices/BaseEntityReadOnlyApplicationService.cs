using AutoMapper;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using DND.Common.Interfaces.Services;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Extensions;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class BaseEntityReadOnlyApplicationService<TEntity, TDto, TDomainService> : BaseApplicationService, IBaseEntityReadOnlyApplicationService<TDto>
          where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
          where TDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
          where TDomainService : IBaseEntityReadOnlyDomainService<TEntity>

    {
        protected virtual TDomainService DomainService { get; }

        public BaseEntityReadOnlyApplicationService(TDomainService domainService, IMapper mapper)
           : base(mapper)
        {
            DomainService = domainService;
        }

        public BaseEntityReadOnlyApplicationService(TDomainService domainService)
        {
            DomainService = domainService;
        }

        public virtual void AddIncludes(List<Expression<Func<TEntity, Object>>> includes)
        {

        }

        public virtual bool IncludeAllCompositionRelationshipProperties => false;
        public virtual bool IncludeAllCompositionAndAggregationRelationshipProperties => false;

        #region GetAll
        public virtual IEnumerable<TDto> GetAll(
        Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
        int? pageNo = null,
        int? pageSize = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = DomainService.GetAll(orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(
            CancellationToken cancellationToken,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await DomainService.GetAllAsync(cancellationToken, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }
        #endregion

        #region Search
        public virtual IEnumerable<TDto> Search(
       string search = "",
       Expression<Func<TDto, bool>> filter = null,
       Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
       int? pageNo = null,
       int? pageSize = null,
       bool includeAllCompositionRelationshipProperties = false,
       bool includeAllCompositionAndAggregationRelationshipProperties = false,
       params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = DomainService.Search(search, filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> SearchAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await DomainService.SearchAsync(cancellationToken, search, filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual int GetSearchCount(
        string search = "",
       Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return DomainService.GetSearchCount(search, filterConverted);
        }

        public virtual async Task<int> GetSearchCountAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await DomainService.GetSearchCountAsync(cancellationToken, search, filterConverted).ConfigureAwait(false);
        }
        #endregion

        #region Get
        public virtual IEnumerable<TDto> Get(
           Expression<Func<TDto, bool>> filter = null,
           Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
           int? pageNo = null,
           int? pageSize = null,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = DomainService.Get(filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var entityList = await DomainService.GetAsync(cancellationToken, filterConverted, orderByConverted, pageNo, pageSize, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual int GetCount(
        Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return DomainService.GetCount(filterConverted);
        }

        public virtual async Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await DomainService.GetCountAsync(cancellationToken, filterConverted).ConfigureAwait(false);
        }

        #endregion

        #region GetOne
        public virtual TDto GetOne(
          Expression<Func<TDto, bool>> filter = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = DomainService.GetOne(filterConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await DomainService.GetOneAsync(cancellationToken, filterConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetFirst
        public virtual TDto GetFirst(
         Expression<Func<TDto, bool>> filter = null,
         Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = DomainService.GetFirst(filterConverted, orderByConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await DomainService.GetFirstAsync(cancellationToken, filterConverted, orderByConverted, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);

            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetById
        public virtual TDto GetById(object id,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = DomainService.GetById(id, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken),
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var bo = await DomainService.GetByIdAsync(id, cancellationToken, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);
            return Mapper.Map<TDto>(bo);
        }
        #endregion

        #region GetByIdWithPagedCollectionProperty
        public virtual TDto GetByIdWithPagedCollectionProperty(object id,
           string collectionExpression,
           string search = "",
           string orderBy = null,
           bool ascending = false,
           int? pageNo = null,
           int? pageSize = null)
        {
            var bo = DomainService.GetByIdWithPagedCollectionProperty(id, collectionExpression, search, orderBy, ascending, pageNo, pageSize);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null,
            int? pageSize = null)
        {

            var bo = await DomainService.GetByIdWithPagedCollectionPropertyAsync(cancellationToken, id, collectionExpression, search, orderBy, ascending, pageNo, pageSize);
            return Mapper.Map<TDto>(bo);
        }

        public int GetByIdWithPagedCollectionPropertyCount(object id,
            string collectionExpression,
            string search = "")
        {
            //var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return DomainService.GetByIdWithPagedCollectionPropertyCount(id, collectionExpression, search);
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken,
            object id,
            string collectionExpression,
            string search = "")
        {
           // var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return await DomainService.GetByIdWithPagedCollectionPropertyCountAsync(cancellationToken, id, collectionExpression, search);
        }
        #endregion

        #region GetByIds
        public virtual IEnumerable<TDto> GetByIds(IEnumerable<object> ids,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var result = DomainService.GetByIds(ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }

        public virtual async Task<IEnumerable<TDto>> GetByIdsAsync(CancellationToken cancellationToken,
         IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);
            var list = includesConverted.ToList();
            AddIncludes(list);
            includesConverted = list.ToArray();

            var result = await DomainService.GetByIdsAsync(cancellationToken, ids, includeAllCompositionRelationshipProperties || IncludeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties || IncludeAllCompositionAndAggregationRelationshipProperties, includesConverted).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return DomainService.Exists(filterConverted);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            )
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await DomainService.ExistsAsync(cancellationToken, filterConverted).ConfigureAwait(false);
        }

        public virtual bool Exists(object id)
        {
            return DomainService.Exists(id);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id
            )
        {
            return await DomainService.ExistsAsync(cancellationToken, id).ConfigureAwait(false);
        }
        #endregion
    }
}
