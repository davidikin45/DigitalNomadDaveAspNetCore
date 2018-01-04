using AutoMapper;
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

namespace Solution.Base.Implementation.Services
{
    public abstract class BaseEntityReadOnlyService<TContext, TEntity, TDto> : BaseBusinessService, IBaseEntityReadOnlyService<TDto>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
          where TDto : class, IBaseEntity
    {

        public BaseEntityReadOnlyService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
           : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public BaseEntityReadOnlyService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        protected virtual IQueryable<TDto> GetQueryable(
          IEnumerable<TDto> list,
          Expression<Func<TDto, bool>> filter = null,
          Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
          int? skip = null,
          int? take = null)
        {
            //includeProperties = includeProperties ?? string.Empty;
            IQueryable<TDto> query = list.AsQueryable();


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

        public virtual IEnumerable<TDto> GetAll(
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            IEnumerable<TDto> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.Repository<TContext, TEntity>().GetAll(orderByConverted, pageNo * pageSize, pageSize, includesConverted);
                dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

                return dtoList;
            }
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

            IEnumerable<TDto> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.Repository<TContext, TEntity>().GetAllAsync(orderByConverted, pageNo * pageSize, pageSize, includesConverted);
                dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

                return dtoList;
            }
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

            IEnumerable<TDto> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                var entityList = unitOfWork.Repository<TContext, TEntity>().Search(search, filterConverted, orderByConverted, pageNo * pageSize, pageSize, includesConverted);

                dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

                return dtoList;
            }

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

            IEnumerable<TDto> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {

                var entityList = await unitOfWork.Repository<TContext, TEntity>().SearchAsync(search, filterConverted, orderByConverted, pageNo * pageSize, pageSize, includesConverted);
                dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

                return dtoList;
            }
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

            IEnumerable<TDto> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                var entityList = unitOfWork.Repository<TContext, TEntity>().Get(filterConverted, orderByConverted, pageNo * pageSize, pageSize, includesConverted);

                dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

                return dtoList;
            }

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

            IEnumerable<TDto> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {

                var entityList = await unitOfWork.Repository<TContext, TEntity>().GetAsync(filterConverted, orderByConverted, pageNo * pageSize, pageSize, includesConverted);
                dtoList = entityList.ToList().Select(Mapper.Map<TEntity, TDto>);

                return dtoList;
            }
        }

        public virtual TDto GetOne(
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                var result = unitOfWork.Repository<TContext, TEntity>().GetOne(filterConverted, includesConverted);

                return Mapper.Map<TDto>(result);
            }
        }

        public virtual async Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await unitOfWork.Repository<TContext, TEntity>().GetOneAsync(filterConverted, includesConverted);
                return Mapper.Map<TDto>(result);
            }

        }

        public virtual TDto GetFirst(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);
            var orderByConverted = GetMappedOrderBy<TDto, TEntity>(orderBy);
            var includesConverted = GetMappedIncludes<TDto, TEntity>(includeProperties);

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {

                var result = unitOfWork.Repository<TContext, TEntity>().GetFirst(filterConverted, orderByConverted, includesConverted);
                return Mapper.Map<TDto>(result);
            }

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

            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
              
                var result = await unitOfWork.Repository<TContext, TEntity>().GetFirstAsync(filterConverted, orderByConverted, includesConverted);
                return Mapper.Map<TDto>(result);
            }
        }

        public virtual TDto GetById(object id)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var result = unitOfWork.Repository<TContext, TEntity>().GetById(id);
                return Mapper.Map<TDto>(result);
            }
        }

        public virtual async Task<TDto> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await unitOfWork.Repository<TContext, TEntity>().GetByIdAsync(id);
                return Mapper.Map<TDto>(result);
            }
        }

        public virtual int GetCount(
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            int count = 0;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                count = unitOfWork.Repository<TContext, TEntity>().GetCount(filterConverted);
            }
            return count;
        }

        public virtual async Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            int count = 0;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await unitOfWork.Repository<TContext, TEntity>().GetCountAsync(filterConverted);
            }
            return count;
        }

        public virtual int GetSearchCount(
            string search = "",
           Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            int count = 0;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                count = unitOfWork.Repository<TContext, TEntity>().GetSearchCount(search, filterConverted);
            }
            return count;
        }

        public virtual async Task<int> GetSearchCountAsync(
            CancellationToken cancellationToken,
             string search = "",
            Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            int count = 0;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await unitOfWork.Repository<TContext, TEntity>().GetSearchCountAsync(search, filterConverted);
            }
            return count;
        }

        public virtual bool Exists(Expression<Func<TDto, bool>> filter = null)
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            bool exists = false;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                exists = unitOfWork.Repository<TContext, TEntity>().GetExists(filterConverted);
            }
            return exists;
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            )
        {
            var filterConverted = GetMappedSelector<TDto, TEntity, bool>(filter);

            bool exists = false;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                exists = await unitOfWork.Repository<TContext, TEntity>().GetExistsAsync(filterConverted);
            }
            return exists;
        }
    }
}
