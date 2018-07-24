using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.ApplicationServices
{
    public interface IBaseEntityReadOnlyApplicationService<TDto> : IBaseApplicationService
        where TDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
    {

        IEnumerable<TDto> GetAll(
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties);

        Task<IEnumerable<TDto>> GetAllAsync(
            CancellationToken cancellationToken,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        IEnumerable<TDto> Search(
         string search = "",
         Expression<Func<TDto, bool>> filter = null,
         Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
         int? pageNo = null,
         int? pageSize = null,
         params Expression<Func<TDto, Object>>[] includeProperties)
         ;

        Task<IEnumerable<TDto>> SearchAsync(
            CancellationToken cancellationToken,
            string search = "",
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        IEnumerable<TDto> Get(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<IEnumerable<TDto>> GetAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            int? pageNo = null,
            int? pageSize = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetOne(
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<TDto> GetOneAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetFirst(
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<TDto> GetFirstAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null,
            Expression<Func<IQueryable<TDto>, IOrderedQueryable<TDto>>> orderBy = null,
            params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetById(object id, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        Task<TDto> GetByIdAsync(object id, 
             CancellationToken cancellationToken, bool includeAllCompositionRelationshipProperties = false, bool includeAllCompositionAndAggregationRelationshipProperties = false, params Expression<Func<TDto, Object>>[] includeProperties)
            ;

        TDto GetByIdWithPagedCollectionProperty(object id, string collectionProperty, int? pageNo = null, int? pageSize = null, object colectionItemId = null);

        Task<TDto> GetByIdWithPagedCollectionPropertyAsync(CancellationToken cancellationToken, object id, string collectionProperty, int? pageNo = null, int? pageSize = null, object colectionItemId = null);

        int GetByIdWithPagedCollectionPropertyCount(object id, string collectionProperty);

        Task<int> GetByIdWithPagedCollectionPropertyCountAsync(CancellationToken cancellationToken, object id, string collectionProperty);

        IEnumerable<TDto> GetByIds(IEnumerable<object> ids)
           ;

        Task<IEnumerable<TDto>> GetByIdsAsync(IEnumerable<object> ids,
             CancellationToken cancellationToken)
            ;

        int GetCount(Expression<Func<TDto, bool>> filter = null)
            ;

        Task<int> GetCountAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null
            )
            ;


        int GetSearchCount(string search = "", Expression<Func<TDto, bool>> filter = null)
            ;

        Task<int> GetSearchCountAsync(
          CancellationToken cancellationToken,
          string search = "",
          Expression<Func<TDto, bool>> filter = null
          )
          ;

        bool Exists(Expression<Func<TDto, bool>> filter = null)
            ;

        Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            Expression<Func<TDto, bool>> filter = null)
            ;

        bool Exists(object id)
           ;

        Task<bool> ExistsAsync(
            CancellationToken cancellationToken,
            object id)
            ;
    }
}
