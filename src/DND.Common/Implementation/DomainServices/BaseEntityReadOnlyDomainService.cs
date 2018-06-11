﻿using DND.Common.Implementation.Validation;
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
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAllNoTracking(orderBy, pageNo * pageSize, pageSize, includeProperties);

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
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetAllNoTrackingAsync(orderBy, pageNo * pageSize, pageSize, includeProperties).ConfigureAwait(false);

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
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().SearchNoTracking(search, filter, orderBy, pageNo * pageSize, pageSize, includeProperties);

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
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().SearchNoTrackingAsync(search, filter, orderBy, pageNo * pageSize, pageSize, includeProperties).ConfigureAwait(false);

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
                var entityList = unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetNoTracking(filter, orderBy, pageNo * pageSize, pageSize, includeProperties);

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
                var entityList = await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetNoTrackingAsync(filter, orderBy, pageNo * pageSize, pageSize, includeProperties).ConfigureAwait(false);

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
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetOneAsync(filter, includeProperties).ConfigureAwait(false);
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
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetFirstAsync(filter, orderBy, includeProperties).ConfigureAwait(false);
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
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdAsync(id).ConfigureAwait(false);
            }
        }

        public virtual IEnumerable<TEntity> GetByIds(IEnumerable<object> ids)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly())
            {
                return unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdsNoTracking(ids);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<object> ids,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<TContext, TEntity>().GetByIdsNoTrackingAsync(ids).ConfigureAwait(false);
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
    }
}
