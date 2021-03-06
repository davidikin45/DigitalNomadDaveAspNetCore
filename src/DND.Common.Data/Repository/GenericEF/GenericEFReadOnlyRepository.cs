﻿using DND.Common.Data.Helpers;
using DND.Common.Infrastructure.Extensions;
using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Data.Repository.GenericEF
{
    public class GenericEFReadOnlyRepository<TEntity> : IGenericEFReadOnlyRepository<TEntity>
   where TEntity : class
    {
        protected readonly DbContext _context;

        //AsNoTracking() causes EF to bypass cache for reads and writes - Ideal for Web Applications and short lived contexts
        public GenericEFReadOnlyRepository(DbContext context)
        {
            this._context = context;
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
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (!tracking)
            {
                query = query.AsNoTracking();
            }
            else
            {
                query = query.AsTracking();
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = CreateSearchQuery(query, search);
            }

            if (includeAllCompositionAndAggregationRelationshipProperties)
            {
                var includesList = GetAllCompositionAndAggregationRelationshipPropertyIncludes(false, typeof(TEntity));

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
                    var includesList = GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, typeof(TEntity));

                    foreach (var include in includesList)
                    {
                        query = query.Include(include);
                    }
                }

                if (includeProperties != null && includeProperties.Count() > 0)
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
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            DebugSQL(query);

            return query;
        }

        private List<string> GetAllCompositionRelationshipPropertyIncludes(Type type, int maxDepth = 10)
        {
            return GetAllCompositionAndAggregationRelationshipPropertyIncludes(true, type, null, 0, maxDepth);
        }

        private List<string> GetAllCompositionAndAggregationRelationshipPropertyIncludes(bool compositionRelationshipsOnly, Type type, string path = null, int depth = 0, int maxDepth = 5)
        {
            List<string> includesList = new List<string>();
            if (depth > maxDepth)
            {
                return includesList;
            }

            List<Type> excludeTypes = new List<Type>()
            {
                typeof(DateTime),
                typeof(String),
                typeof(byte[])
           };

            IEnumerable<PropertyInfo> properties = type.GetProperties().Where(p => p.CanWrite && !p.PropertyType.IsValueType && !excludeTypes.Contains(p.PropertyType) && ((!compositionRelationshipsOnly && !p.PropertyType.IsCollection()) || (p.PropertyType.IsCollection() && type != p.PropertyType.GetGenericArguments().First()))).ToList();

            foreach (var p in properties)
            {
                var includePath = !string.IsNullOrWhiteSpace(path) ? path + "." + p.Name : p.Name;

                includesList.Add(includePath);

                Type propType = null;
                if (p.PropertyType.IsCollection())
                {
                    propType = type.GetGenericArguments(p.Name).First();
                }
                else
                {
                    propType = p.PropertyType;
                }

                includesList.AddRange(GetAllCompositionAndAggregationRelationshipPropertyIncludes(compositionRelationshipsOnly, propType, includePath, depth + 1, maxDepth));
            }

            return includesList;
        }

        private IQueryable<T> CreateSearchQuery<T>(IQueryable<T> query, string values)
        {
            List<Expression> andExpressions = new List<Expression>();

            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");

            MethodInfo contains_method = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var ignore = new List<string>() { "" };

            foreach (var value in values.Split('&'))
            {
                List<Expression> orExpressions = new List<Expression>();

                foreach (PropertyInfo prop in typeof(T).GetProperties().Where(x => x.PropertyType == typeof(string) && !ignore.Contains(x.Name)))
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

            Expression<Func<T, bool>> expression = Expression.Lambda<Func<T, bool>>(
                and_expression, parameter);

            return query.Where(expression);
        }

        private void DebugSQL(IQueryable<TEntity> query)
        {
            var sql = query.ToString();
        }

        #region SQLQuery
        public virtual IEnumerable<TEntity> SQLQuery(string query, params object[] paramaters)
        {
            return _context.Set<TEntity>().AsNoTracking().FromSql(query, paramaters).ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> SQLQueryAsync(string query, params object[] paramaters)
        {
            return await _context.Set<TEntity>().AsNoTracking().FromSql(query, paramaters).ToListAsync();
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

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken,          
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, null, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<IEnumerable<TEntity>> GetAllNoTrackingAsync(CancellationToken cancellationToken,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, null, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<IEnumerable<TEntity>> GetAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<IEnumerable<TEntity>> GetNoTrackingAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            //string includeProperties = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, filter).Count();
        }

        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, filter).CountAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<IEnumerable<TEntity>> SearchAsync(CancellationToken cancellationToken,
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
            return await GetQueryable(true, search, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<IEnumerable<TEntity>> SearchNoTrackingAsync(CancellationToken cancellationToken,
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
            return await GetQueryable(false, search, filter, orderBy, skip, take, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, search, filter).Count();
        }

        public virtual async Task<int> GetSearchCountAsync(CancellationToken cancellationToken, string search = "", Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, search, filter).CountAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<TEntity> GetOneAsync(CancellationToken cancellationToken,
          Expression<Func<TEntity, bool>> filter = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual TEntity GetOneNoTracking(
         Expression<Func<TEntity, bool>> filter = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
        }

        public virtual async Task<TEntity> GetOneNoTrackingAsync(CancellationToken cancellationToken,
          Expression<Func<TEntity, bool>> filter = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<TEntity> GetFirstAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(true, null, filter, orderBy, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
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

        public virtual async Task<TEntity> GetFirstNoTrackingAsync(CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            return await GetQueryable(false, null, filter, orderBy, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region GetById
        public virtual TEntity GetById(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            if(includeAllCompositionRelationshipProperties || includeAllCompositionAndAggregationRelationshipProperties || (includeProperties != null && includeProperties.Count() > 0))
            {
                //return _context.FindEntityById<TEntity>(id);
                Expression<Func<TEntity, bool>> filter = SearchForEntityById<TEntity>(id);
                return GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
            }
            else
            {
                return _context.Set<TEntity>().Find(id);
            }      
        }

        public virtual TEntity GetByIdNoTracking(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = SearchForEntityById<TEntity>(id);
            return GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefault();
        }

        public async virtual Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            if (includeAllCompositionRelationshipProperties || includeAllCompositionAndAggregationRelationshipProperties || (includeProperties != null && includeProperties.Count() > 0))
            {
                Expression<Func<TEntity, bool>> filter = SearchForEntityById<TEntity>(id);
                return await GetQueryable(true, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                return await _context.Set<TEntity>().FindAsync(id);
            }
        }

        public async virtual Task<TEntity> GetByIdNoTrackingAsync(CancellationToken cancellationToken, object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            Expression<Func<TEntity, bool>> filter = SearchForEntityById<TEntity>(id);
            return await GetQueryable(false, null, filter, null, null, null, includeAllCompositionRelationshipProperties, includeAllCompositionAndAggregationRelationshipProperties, includeProperties).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }

        public static Expression<Func<TEntity, bool>> SearchForEntityById<TEntity>(object id)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.PropertyOrField(item, "Id");
            var propType = typeof(TEntity).GetProperty("Id").PropertyType;

            var value = Expression.Constant((dynamic)Convert.ChangeType(id, propType));

            var equal = Expression.Equal(prop, value);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, item);
            return lambda;
        }
        #endregion

        #region GetByIdWithPagedCollectionProperty
        public virtual TEntity GetByIdWithPagedCollectionProperty(object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null,
            int? take = null)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                _context.LoadCollectionProperty(entity, collectionExpression, search, orderBy, ascending, skip, take);
            }
            return entity;
        }

        public async virtual Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, object id,
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? skip = null,
            int? take = null)
        {
            var entity = await GetByIdAsync(cancellationToken, id);
            if (entity != null)
            {
                await _context.LoadCollectionPropertyAsync(entity, collectionExpression, search, orderBy, ascending, skip, take, cancellationToken);
            }
            return entity;
        }

        public virtual int GetByIdWithPagedCollectionPropertyCount(object id, string collectionExpression, string search = "")
        {
            var entity = GetById(id);
            if (entity != null)
            {
                return _context.CollectionPropertyCount(entity, collectionExpression, search);
            }
            return 0;
        }

        public virtual async Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, object id, string collectionExpression, string search = "")
        {
            var entity = await GetByIdAsync(cancellationToken, id);
            if (entity != null)
            {
                return await _context.CollectionPropertyCountAsync(entity, collectionExpression, search, cancellationToken);
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

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds<TEntity>(list);
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

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds<TEntity>(list);
            return GetQueryable(false, null, filter, null, null).ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> GetByIdsAsync(CancellationToken cancellationToken, IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds<TEntity>(list);
            return await GetQueryable(false, null, filter, null, null).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public async virtual Task<IEnumerable<TEntity>> GetByIdsNoTrackingAsync(CancellationToken cancellationToken, IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
        {
            var list = new List<object>();
            foreach (object id in ids)
            {
                list.Add(id);
            }

            Expression<Func<TEntity, bool>> filter = SearchForEntityByIds<TEntity>(list);
            return await GetQueryable(false, null, filter, null, null).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public static Expression<Func<TEntity, bool>> SearchForEntityByIds<TEntity>(IEnumerable<object> ids)
        {
            var item = Expression.Parameter(typeof(TEntity), "entity");
            var prop = Expression.PropertyOrField(item, "Id");

            var propType = typeof(TEntity).GetProperty("Id").PropertyType;

            var genericType = typeof(List<>).MakeGenericType(propType);
            var idList = Activator.CreateInstance(genericType);

            var add_method = idList.GetType().GetMethod("Add");
            foreach (var id in ids)
            {
                add_method.Invoke(idList, new object[] { (dynamic)Convert.ChangeType(id, propType) });
            }

            var contains_method = idList.GetType().GetMethod("Contains");
            var value_expression = Expression.Constant(idList);
            var contains_expression = Expression.Call(value_expression, contains_method, prop);
            var lamda = Expression.Lambda<Func<TEntity, bool>>(contains_expression, item);
            return lamda;
        }
        #endregion

        #region Exists
        public virtual bool Exists(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(true, null, filter).ToList().Any();
        }

        public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>> filter = null)
        {
            return (await GetQueryable(true, null, filter).ToListAsync(cancellationToken).ConfigureAwait(false)).Any();
        }

        public virtual bool ExistsNoTracking(Expression<Func<TEntity, bool>> filter = null)
        {
            return GetQueryable(false, null, filter).Any();
        }

        public virtual async Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, Expression<Func<TEntity, bool>> filter = null)
        {
            return await GetQueryable(false, null, filter).AnyAsync(cancellationToken).ConfigureAwait(false);
        }

        public bool Exists(object id)
        {
            return GetById(id) != null;
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken, object id)
        {
            return (await GetByIdAsync(cancellationToken, id)) != null;
        }

        public bool ExistsNoTracking(object id)
        {
            return GetByIdNoTracking(id) != null;
        }

        public async Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, object id)
        {
            return (await GetByIdNoTrackingAsync(cancellationToken, id)) != null;
        }

        public virtual bool Exists(TEntity entity)
        {
            return _context.EntityExists(entity);
        }

        public virtual async Task<bool> ExistsAsync(CancellationToken cancellationToken, TEntity entity)
        {
            return await _context.EntityExistsAsync(entity, cancellationToken);
        }

        public virtual bool ExistsNoTracking(TEntity entity)
        {
            return _context.EntityExistsNoTracking(entity);
        }

        public virtual async Task<bool> ExistsNoTrackingAsync(CancellationToken cancellationToken, TEntity entity)
        {
            return await _context.EntityExistsNoTrackingAsync(entity, cancellationToken);
        }


        public virtual bool ExistsById(object id)
        {
            return _context.EntityExistsById<TEntity>(id);
        }

        public virtual async Task<bool> ExistsByIdAsync(CancellationToken cancellationToken, object id)
        {
            return await _context.EntityExistsByIdAsync<TEntity>(id, cancellationToken).ConfigureAwait(false);
        }

        public virtual bool ExistsByIdNoTracking(object id)
        {
            return _context.EntityExistsByIdNoTracking<TEntity>(id);
        }

        public virtual async Task<bool> ExistsByIdNoTrackingAsync(CancellationToken cancellationToken, object id)
        {
            return await _context.EntityExistsByIdNoTrackingAsync<TEntity>(id, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region Validate

        public Result Validate(TEntity entity, ValidationMode mode)
        {
            var task = ValidateAsync(default(CancellationToken), entity, mode);
            task.Wait();
            return task.Result;
        }

        public async Task<Result> ValidateAsync(CancellationToken cancellationToken, TEntity entity, ValidationMode mode)
        {
            var validatableObject = entity as IObjectValidatable;
            if (validatableObject != null)
            {
                if (mode != ValidationMode.Delete)
                {

                    var objectValidationErrors = validatableObject.Validate().ToList();
                    if (objectValidationErrors.Any())
                    {
                        return Result.ObjectValidationFail(objectValidationErrors);
                    }

                }
            }

            var entityObject = entity as IEntity;
            if (entityObject != null)
            {
                if (mode == ValidationMode.Insert || mode == ValidationMode.Update || mode == ValidationMode.Delete)
                {
                    var dbDependantValidationErrors = await entityObject.ValidateWithDbConnectionAsync(_context, mode).ConfigureAwait(false);
                    if (dbDependantValidationErrors.Any())
                    {
                        return Result.ObjectValidationFail(dbDependantValidationErrors);
                    }
                }
            }

            return Result.Ok();
        }
        #endregion
    }
}
