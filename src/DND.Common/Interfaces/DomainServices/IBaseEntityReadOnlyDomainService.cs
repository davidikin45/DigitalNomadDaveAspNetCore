﻿using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.DomainServices
{
    public interface IBaseEntityReadOnlyDomainService<TEntity> : IBaseDomainService
        where TEntity : class, IBaseEntity
    {

        IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetAllAsync(
            CancellationToken cancellationToken,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> Search(
         string search = "",
         Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         int? pageNo = null,
         int? pageSize = null,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
         ;

        Task<IEnumerable<TEntity>> SearchAsync(
            CancellationToken cancellationToken,
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<IEnumerable<TEntity>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetById(object id, 
            bool includeAllCompositionRelationshipProperties = false, 
            bool includeAllCompositionAndAggregationRelationshipProperties = false, 
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetByIdWithPagedCollectionProperty(object id, 
            string collectionProperty,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null, 
            object collectionItemId = null);

        int GetByIdWithPagedCollectionPropertyCount(object id, 
            string collectionProperty,
            string search = "");

        Task<TEntity> GetByIdAsync(object id,
             CancellationToken cancellationToken, 
             bool includeAllCompositionRelationshipProperties = false, 
             bool includeAllCompositionAndAggregationRelationshipProperties = false, 
             params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionProperty,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null, 
            object collectionItemId = null);

        Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionProperty,
            string search = "");

        IEnumerable<TEntity> GetByIds(IEnumerable<object> ids,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
           ;

        Task<IEnumerable<TEntity>> GetByIdsAsync(CancellationToken cancellationToken,
            IEnumerable<object> ids,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        int GetCount(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null
            )
            ;

        int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<int> GetSearchCountAsync(
          CancellationToken cancellationToken,
          string search = "",
          Expression<Func<TEntity, bool>> filter = null
          )
          ;

        bool Exists(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TEntity, bool>> filter = null)
            ;

        bool Exists(object id)
           ;

        Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id)
            ;
    }
}
