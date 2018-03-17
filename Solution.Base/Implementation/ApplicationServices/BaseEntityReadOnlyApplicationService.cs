using AutoMapper;
using Solution.Base.Interfaces.ApplicationServices;
using Solution.Base.Interfaces.DomainServices;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.Services;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.ApplicationServices
{
    public abstract class BaseEntityReadOnlyApplicationService<TContext, TEntity, TDto> : BaseApplicationService, IBaseEntityReadOnlyApplicationService<TDto>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
          where TDto : class, IBaseEntity
    {
        protected virtual IBaseEntityReadOnlyDomainService<TEntity> ReadOnlyDomainService { get; }

        public BaseEntityReadOnlyApplicationService(IBaseEntityReadOnlyDomainService<TEntity> domainService, IMapper mapper)
           : base(mapper)
        {
            ReadOnlyDomainService = domainService;
        }

        public BaseEntityReadOnlyApplicationService(IBaseEntityReadOnlyDomainService<TEntity> domainService)
        {
            ReadOnlyDomainService = domainService;
        }

        public virtual IEnumerable<TDto> GetAll(
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var entityList = ReadOnlyDomainService.GetAll(orderByConverted, pageNo, pageSize, includesConverted);

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

            var entityList = await ReadOnlyDomainService.GetAllAsync(cancellationToken, orderByConverted, pageNo, pageSize, includesConverted);

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

            var entityList = ReadOnlyDomainService.Search(search, filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

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

            var entityList = await ReadOnlyDomainService.SearchAsync(cancellationToken, search, filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

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

            var entityList = ReadOnlyDomainService.Get(filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

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

            var entityList = await ReadOnlyDomainService.GetAsync(cancellationToken, filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

            IEnumerable<TDto> dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

            return dtoList;
        }

        public virtual TDto GetOne(
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = ReadOnlyDomainService.GetOne(filterConverted, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            var bo = await ReadOnlyDomainService.GetOneAsync(cancellationToken, filterConverted, includesConverted);

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

            var bo = ReadOnlyDomainService.GetFirst(filterConverted, orderByConverted, includesConverted);

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

            var bo = await ReadOnlyDomainService.GetFirstAsync(cancellationToken, filterConverted, orderByConverted, includesConverted);

            return Mapper.Map<TDto>(bo);
        }

        public virtual TDto GetById(object id)
        {
            var bo = ReadOnlyDomainService.GetById(id);
            return Mapper.Map<TDto>(bo);
        }

        public virtual async Task<TDto> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bo = await ReadOnlyDomainService.GetByIdAsync(id, cancellationToken);
            return Mapper.Map<TDto>(bo);
        }

        public virtual IEnumerable<TDto> GetById(IEnumerable<object> ids)
        {
            var result = ReadOnlyDomainService.GetById(ids);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }

        public virtual async Task<IEnumerable<TDto>> GetByIdAsync(IEnumerable<object> ids,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await ReadOnlyDomainService.GetByIdAsync(ids, cancellationToken);
            return Mapper.Map<IEnumerable<TDto>>(result);
        }

        public virtual int GetCount(
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return ReadOnlyDomainService.GetCount(filterConverted);
        }

        public virtual async Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await ReadOnlyDomainService.GetCountAsync(cancellationToken, filterConverted);
        }

        public virtual int GetSearchCount(
            string search = "",
           Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return ReadOnlyDomainService.GetSearchCount(search, filterConverted);
        }

        public virtual async Task<int> GetSearchCountAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await ReadOnlyDomainService.GetSearchCountAsync(cancellationToken, search, filterConverted);
        }

        public virtual bool Exists(Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return ReadOnlyDomainService.Exists(filterConverted);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            )
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            return await ReadOnlyDomainService.ExistsAsync(cancellationToken, filterConverted);
        }

        public virtual bool Exists(object id)
        {
            return ReadOnlyDomainService.Exists(id);
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id
            )
        {
            return await ReadOnlyDomainService.ExistsAsync(cancellationToken, id);
        }

    }
}
