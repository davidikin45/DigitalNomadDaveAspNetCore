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
    public interface IBaseReadOnlyRepository<TEntity>
            where TEntity : class, IBaseEntity
    {

        IEnumerable<TEntity> SQLQuery(string query, params object[] paramaters);
        Task<IEnumerable<TEntity>> SQLQueryAsync(string query, params object[] paramaters);

        IEnumerable<TEntity> GetAll(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, Object>>[] includeProperties);

    Task<IEnumerable<TEntity>> GetAllAsync(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> Search(
             string search = "",
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           int? skip = null,
           int? take = null,
           params Expression<Func<TEntity, Object>>[] includeProperties)
           ;

        Task<IEnumerable<TEntity>> SearchAsync(
            string search = "",
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int? skip = null,
            int? take = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetOne(
            Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetOneAsync(
            Expression<Func<TEntity, bool>> filter = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetFirst(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        Task<TEntity> GetFirstAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            params Expression<Func<TEntity, Object>>[] includeProperties)
            ;

        TEntity GetById(object id)
            ;

        Task<TEntity> GetByIdAsync(object id)
            ;

        IEnumerable<TEntity> GetById(IEnumerable<object> ids)
           ;

        Task<IEnumerable<TEntity>> GetByIdAsync(IEnumerable<object> ids)
            ;

        int GetCount(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
            ;

        int GetSearchCount(string search = "", Expression<Func<TEntity, bool>> filter = null)
          ;

        Task<int> GetSearchCountAsync(string search = "", Expression<Func<TEntity, bool>> filter = null)
            ;


        bool GetExists(Expression<Func<TEntity, bool>> filter = null)
            ;

        Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> filter = null)
            ;

        Result Validate(TEntity entity, ValidationMode mode);

        Task<Result> ValidateAsync(TEntity entity, ValidationMode mode);
    }

}
