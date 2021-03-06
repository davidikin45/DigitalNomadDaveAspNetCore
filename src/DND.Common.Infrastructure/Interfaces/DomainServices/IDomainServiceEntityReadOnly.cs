﻿using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure.Interfaces.DomainServices
{
    public interface IDomainServiceEntityReadOnly<TEntity> : IDomainService
        where TEntity : class
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
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null);

        int GetByIdWithPagedCollectionPropertyCount(object id, 
            string collectionExpression,
            string search = "");

        Task<TEntity> GetByIdAsync(object id,
             CancellationToken cancellationToken, 
             bool includeAllCompositionRelationshipProperties = false, 
             bool includeAllCompositionAndAggregationRelationshipProperties = false, 
             params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionExpression,
            string search = "",
            string orderBy = null,
            bool ascending = false,
            int? pageNo = null, 
            int? pageSize = null);

        Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, 
            object id, 
            string collectionExpression,
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

        Result Validate(TEntity entity, ValidationMode mode);

        Task<Result> ValidateAsync(CancellationToken cancellationToken, TEntity entity, ValidationMode mode);
    }
}
