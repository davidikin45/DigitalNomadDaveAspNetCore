﻿using System;
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

namespace DND.Common.Implementation.Repository.EntityFramework
{
    public class GenericEFReadOnlyRepository<TEntity> : IBaseReadOnlyRepository<TEntity>
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
                query = CreateSearchQuery(query, search);
            }

            if (includeProperties != null)
            {
                foreach (var includeExpression in includeProperties)
                {
                    query = query.Include(includeExpression);
                }
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            DebugSQL(query);

            return query;
        }

        private void DebugSQL(IQueryable<TEntity> query)
        {
            var sql = query.ToString();
        }

        protected virtual IQueryable<TEntity> CreateSearchQuery(IQueryable<TEntity> query, string values)
        {

            List<Expression> andExpressions = new List<Expression>();

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p");

            MethodInfo contains_method = typeof(string).GetMethod("Contains", new[] { typeof(string) });


            foreach (var value in values.Split('&'))
            {
                List<Expression> orExpressions = new List<Expression>();

                foreach (PropertyInfo prop in typeof(TEntity).GetProperties().Where(x => x.PropertyType == typeof(string)))
                {
                    MemberExpression member_expression = Expression.PropertyOrField(parameter, prop.Name);

                    ConstantExpression value_expression = Expression.Constant(value, typeof(string));

                    MethodCallExpression contains_expression = Expression.Call(member_expression, contains_method, value_expression);

                    orExpressions.Add(contains_expression);
                }

                if (orExpressions.Count == 0)
                    return query;

                Expression or_expression = orExpressions[0];

                for (int i = 1; i < orExpressions.Count; i++)
                {
                    or_expression = Expression.OrElse(or_expression, orExpressions[i]);
                }

                andExpressions.Add(or_expression);
            }

            if (andExpressions.Count == 0)
                return query;

            Expression and_expression = andExpressions[0];

            for (int i = 1; i < andExpressions.Count; i++)
            {
                and_expression = Expression.AndAlso(and_expression, andExpressions[i]);
            }

            Expression<Func<TEntity, bool>> expression = Expression.Lambda<Func<TEntity, bool>>(
                and_expression, parameter);

            return query.Where(expression);
        }

        public virtual IEnumerable<TEntity> GetAll(
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          int? skip = null,
          int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, null, orderBy, skip, take, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            // string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, orderBy, skip, take, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> GetAllNoTracking(
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        int? skip = null,
        int? take = null,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, null, orderBy, skip, take, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllNoTrackingAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            // string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, orderBy, skip, take, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> SQLQuery(string query, params object[] paramaters)
        {
            return _context.SQLQueryNoTracking<TEntity>(query, paramaters);
        }

        public async virtual Task<IEnumerable<TEntity>> SQLQueryAsync(string query, params object[] paramaters)
        {
            return await _context.SQLQueryNoTrackingAsync<TEntity>(query, paramaters).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //  string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, filter, orderBy, skip, take, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, orderBy, skip, take, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> GetNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, orderBy, skip, take, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, orderBy, skip, take, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> Search(
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //  string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, search, filter, orderBy, skip, take, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, search, filter, orderBy, skip, take, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> SearchNoTracking(
          string search = "",
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //  string includeProperties = null,
          int? skip = null,
          int? take = null,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, search, filter, orderBy, skip, take, includeProperties).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> SearchNoTrackingAsync(
             string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, search, filter, orderBy, skip, take, includeProperties).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
          // string includeProperties = "",
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, filter, null, null, null, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
          // string includeProperties = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, null, null, null, includeProperties).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetOneNoTracking(
           Expression<Func<TEntity, bool>> filter = null,
         // string includeProperties = "",
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, null, null, null, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
          // string includeProperties = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, null, null, null, includeProperties).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetFirst(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          //string includeProperties = "",
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(true, null, filter, orderBy, null, null, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          // string includeProperties = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, orderBy, null, null, includeProperties).FirstOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetFirstNoTracking(
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        //string includeProperties = "",
        params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, orderBy, null, null, includeProperties).FirstOrDefault();
        }

        public virtual async Task<TEntity> GetFirstNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          // string includeProperties = null,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, orderBy, null, null, includeProperties).FirstOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetById(object id)
        {
            return _context.FindEntityById<TEntity>(id);
        }

        public virtual TEntity GetByIdNoTracking(object id)
        {
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);
            return GetQueryable(false, null, filter, null, null).SingleOrDefault();
        }

        public async virtual Task<TEntity> GetByIdAsync(object id)
        {
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);
            return await GetQueryable(true, null, filter, null, null).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public async virtual Task<TEntity> GetByIdNoTrackingAsync(object id)
        {
            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityById<TEntity>(id);
            return await GetQueryable(false, null, filter, null, null).SingleOrDefaultAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual IEnumerable<TEntity> GetByIds(IEnumerable<object> ids)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return GetQueryable(true, null, filter, null, null).ToList();
        }

        public virtual IEnumerable<TEntity> GetByIdsNoTracking(IEnumerable<object> ids)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return GetQueryable(false, null, filter, null, null).ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<object> ids)
        {
            var list = new List<object>();
            foreach(object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return await GetQueryable(false, null, filter, null, null).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public async virtual Task<IEnumerable<TEntity>> GetByIdsNoTrackingAsync(IEnumerable<object> ids)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = LamdaHelper.SearchForEntityByIds<TEntity>(list);
            return await GetQueryable(false, null, filter, null, null).ToListAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, filter).Count();
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, filter).CountAsync(_cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, search, filter).Count();
        }

        public virtual async Task<int> GetSearchCountAsync(string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, search, filter).CountAsync(_cancellationToken).ConfigureAwait(false);
        }

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
    }
}
