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

        public virtual IEnumerable<TDto> GetAll(
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = DomainService.GetAll(orderByConverted, pageNo, pageSize, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;        
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(
            CancellationToken cancellationToken, 
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = await DomainService.GetAllAsync(cancellationToken, orderByConverted, pageNo, pageSize, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual IEnumerable<TDto> Search(
       string search = "",
       Expression<Func<TDto, bool>> filter = null,
       Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
       int? pageNo = null,
       int? pageSize = null,
       params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = DomainService.Search(search, filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

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
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = await DomainService.SearchAsync(cancellationToken, search, filterConverted, orderByConverted, pageNo, pageSize, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual IEnumerable<TDto> Get(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = DomainService.Get(filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual async Task<IEnumerable<TDto>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = await DomainService.GetAsync(cancellationToken, filterConverted, orderByConverted, pageNo, pageSize, includesConverted).ConfigureAwait(false);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual TDto GetOne(
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = DomainService.GetOne(filterConverted, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = await DomainService.GetOneAsync(cancellationToken, filterConverted, includesConverted).ConfigureAwait(false);

            return Mapper.Map<TDto>(bo);
        }

        public virtual TDto GetFirst(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = DomainService.GetFirst(filterConverted, orderByConverted, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = await DomainService.GetFirstAsync(cancellationToken, filterConverted, orderByConverted, includesConverted).ConfigureAwait(false);

            return Mapper.Map<TDto>(bo);
        }

        public virtual TDto GetById(object id, params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = DomainService.GetById(id, includesConverted);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken), params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = await DomainService.GetByIdAsync(id, cancellationToken, includesConverted).ConfigureAwait(false);
            return Mapper.Map<TDto>(bo);
        }

        public virtual TDto GetByIdWithPagedCollectionProperty(object id, string collectionProperty, int? pageNo = null, int? pageSize = null, object collectionItemId = null)
        {
            var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            var bo = DomainService.GetByIdWithPagedCollectionProperty(id, mappedCollectionProperty, pageNo, pageSize, collectionItemId);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, object id, string collectionProperty, int? pageNo = null, int? pageSize = null, object collectionItemId = null)
        {
            var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            var bo = await DomainService.GetByIdWithPagedCollectionPropertyAsync(cancellationToken, id, mappedCollectionProperty, pageNo, pageSize, collectionItemId);
            return Mapper.Map<TDto>(bo);
        }

        public int GetByIdWithPagedCollectionPropertyCount(object id, string collectionProperty)
        {
            var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return DomainService.GetByIdWithPagedCollectionPropertyCount(id, mappedCollectionProperty);
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, object id, string collectionProperty)
        {
            var mappedCollectionProperty = Mapper.GetDestinationMappedProperty<TDto, TEntity>(collectionProperty).Name;
            return await DomainService.GetByIdWithPagedCollectionPropertyCountAsync(cancellationToken, id, mappedCollectionProperty);
        }

        public virtual IEnumerable<TDto> GetByIds(IEnumerable<object> ids)
        {
            var result = DomainService.GetByIds(ids);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }

        public virtual async Task<IEnumerable<TDto>> GetByIdsAsync(IEnumerable<object> ids,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await DomainService.GetByIdsAsync(ids, cancellationToken).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TDto>>(result);
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

    }
}
