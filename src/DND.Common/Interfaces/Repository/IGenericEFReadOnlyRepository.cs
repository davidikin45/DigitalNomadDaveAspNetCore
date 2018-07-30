using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DND.Common.Interfaces.Models;
using System.Data.Entity.Core.Objects;
using DND.Common.Implementation.Validation;
using DND.Common.Enums;
using System.Threading;

namespace DND.Common.Interfaces.Repository
{
    public interface IGenericEFReadOnlyRepository<TEntity>
            where TEntity : class, IBaseEntity
    {

        IEnumerable<TEntity> SQLQuery(string query, params object[] paramaters);
        Task<IEnumerable<TEntity>> SQLQueryAsync(string query, params object[] paramaters);

        IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetAllAsync(
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                int? skip = null,
                int? take = null,
                bool includeAllCompositionRelationshipProperties = false,
                bool includeAllCompositionAndAggregationRelationshipProperties = false,
                params Expression<Func<TEntity, Object>>[] includeProperties)
                ;

        IEnumerable<TEntity> GetAllNoTracking(
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties);

        Task<IEnumerable<TEntity>> GetAllNoTrackingAsync(
                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                int? skip = null,
                int? take = null,
                bool includeAllCompositionRelationshipProperties = false,
                bool includeAllCompositionAndAggregationRelationshipProperties = false,
                params Expression<Func<TEntity, Object>>[] includeProperties)
                ;

        IEnumerable<TEntity> Search(
             string search = "",
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           int? skip = null,
           int? take = null,
           bool includeAllCompositionRelationshipProperties = false,
           bool includeAllCompositionAndAggregationRelationshipProperties = false,
           params Expression<Func<TEntity, Object>>[] includeProperties)
           ;

        Task<IEnumerable<TEntity>> SearchAsync(
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> SearchNoTracking(
         string search = "",
       Expression<Func<TEntity, bool>> filter = null,
       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
       int? skip = null,
       int? take = null,
       bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
       params Expression<Func<TEntity, Object>>[] includeProperties)
       ;

        Task<IEnumerable<TEntity>> SearchNoTrackingAsync(
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> GetNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          int? skip = null,
          int? take = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
          ;

        Task<IEnumerable<TEntity>> GetNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
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
            Expression<Func<TEntity, bool>> filter = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetOneNoTracking(
        Expression<Func<TEntity, bool>> filter = null,
        bool includeAllCompositionRelationshipProperties = false,
        bool includeAllCompositionAndAggregationRelationshipProperties = false,
        params Expression<Func<TEntity, Object>>[] includeProperties)
        ;

        Task<TEntity> GetOneNoTrackingAsync(
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
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetFirstNoTracking(
          Expression<Func<TEntity, bool>> filter = null,
          Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
          bool includeAllCompositionRelationshipProperties = false,
          bool includeAllCompositionAndAggregationRelationshipProperties = false,
          params Expression<Func<TEntity, Object>>[] includeProperties)
          ;

        Task<TEntity> GetFirstNoTrackingAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            bool includeAllCompositionRelationshipProperties = false,
            bool includeAllCompositionAndAggregationRelationshipProperties = false,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetById(object id, bool includeAllCompositionRelationshipProperties = false, 
            bool includeAllCompositionAndAggregationRelationshipProperties = false, 
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetByIdAsync(object id, 
            bool includeAllCompositionRelationshipProperties = false, 
            bool includeAllCompositionAndAggregationRelationshipProperties = false, 
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;


        TEntity GetByIdNoTracking(object id, 
            bool includeAllCompositionRelationshipProperties = false, 
            bool includeAllCompositionAndAggregationRelationshipProperties = false, 
            params Expression<Func<TEntity, Object>>[] includeProperties)
          ;

        Task<TEntity> GetByIdNoTrackingAsync(object id, 
            bool includeAllCompositionRelationshipProperties = false, 
            bool includeAllCompositionAndAggregationRelationshipProperties = false, 
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetByIdWithPagedCollectionProperty(object id, 
            string collectionProperty, 
            int? skip = null, 
            int? take = null, 
            object collectionItemId = null);

        Task<TEntity> GetByIdWithPagedCollectionPropertyAsync(object id, 
            string collectionProperty, 
            int? skip = null, 
            int? take = null, 
            object collectionItemId = null);

        int GetByIdWithPagedCollectionPropertyCount(object id, 
            string collectionProperty);

        Task<int> GetByIdWithPagedCollectionPropertyCountAsync(object id, 
            string collectionProperty);

        IEnumerable<TEntity> GetByIds(IEnumerable<object> ids, 
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
           ;

        Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> GetByIdsNoTracking(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
       ;

        Task<IEnumerable<TEntity>> GetByIdsNoTrackingAsync(IEnumerable<object> ids,
         bool includeAllCompositionRelationshipProperties = false,
         bool includeAllCompositionAndAggregationRelationshipProperties = false,
         params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        int GetCount(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
            ;

        int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
          ;

        Task<int> GetSearchCountAsync(string search = "", Expression<Func<TEntity, bool>> filter = null)
            ;


        bool Exists(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> filter = null)
            ;

        bool ExistsNoTracking(Expression<Func<TEntity, bool>> filter = null)
          ;

        Task<bool> ExistsNoTrackingAsync(Expression<Func<TEntity, bool>> filter = null)
            ;

        bool Exists(TEntity entity)
          ;

        Task<bool> ExistsAsync(TEntity entity)
            ;

        bool ExistsNoTracking(TEntity entity)
          ;

        Task<bool> ExistsNoTrackingAsync(TEntity entity)
            ;

        bool ExistsById(object id)
          ;

        Task<bool> ExistsByIdAsync(object id)
            ;

        bool ExistsByIdNoTracking(object id)
          ;

        Task<bool> ExistsByIdNoTrackingAsync(object id)
            ;

        Result Validate(TEntity entity, ValidationMode mode);

        Task<Result> ValidateAsync(TEntity entity, ValidationMode mode);
    }

}
