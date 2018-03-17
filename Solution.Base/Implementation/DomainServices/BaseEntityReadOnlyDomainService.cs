using Solution.Base.Interfaces.DomainServices;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.DomainServices
{
    public abstract class BaseEntityReadOnlyDomainService<TContext, TEntity> : BaseDomainService, IBaseEntityReadOnlyDomainService<TEntity>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()
    {

        public BaseEntityReadOnlyDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

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

        public virtual IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAll(orderBy, pageNo * pageSize, pageSize, includeProperties);

                return entityList;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAllAsync(orderBy, pageNo * pageSize, pageSize, includeProperties);

                return entityList;
            }
        }

        public virtual IEnumerable<TEntity> Search(
       string search = "",
       Expression<Func<TEntity, bool>> filter = null,
       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
       int? pageNo = null,
       int? pageSize = null,
       params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().Search(search, filter, orderBy, pageNo * pageSize, pageSize, includeProperties);

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
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().SearchAsync(search, filter, orderBy, pageNo * pageSize, pageSize, includeProperties);

                return entityList;
            }
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().Get(filter, orderBy, pageNo * pageSize, pageSize, includeProperties);

                return entityList;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAsync(filter, orderBy, pageNo * pageSize, pageSize, includeProperties);

                return entityList;
            }
        }

        public virtual TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetOne(filter, includeProperties);
            }
        }

        public virtual async Task<TEntity> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetOneAsync(filter, includeProperties);
            }
        }

        public virtual TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetFirst(filter, orderBy, includeProperties);
            }
        }

        public virtual async Task<TEntity> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {             
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetFirstAsync(filter, orderBy, includeProperties);
            }
        }

        public virtual TEntity GetById(object id)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetById(id);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(id);
            }
        }

        public virtual IEnumerable<TEntity> GetById(IEnumerable<object> ids)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetById(ids);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetByIdAsync(IEnumerable<object> ids,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(ids);
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
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetCountAsync(filter);
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
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetSearchCountAsync(search, filter);
            }
        }

        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetExists(filter);
            }
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null
            )
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetExistsAsync(filter);
            }
        }

        public virtual bool Exists(object id)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetExists(x => x.Id.ToString() == id.ToString());
            }
        }

        public virtual async Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id
            )
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetExistsAsync(x => x.Id.ToString() == id.ToString());
            }
        }
    }
}
