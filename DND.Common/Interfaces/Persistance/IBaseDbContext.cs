using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Linq.Expressions;
using RefactorThis.GraphDiff;
using Microsoft.AspNet.Identity.EntityFramework;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Models;

namespace DND.Common.Interfaces.Persistance
{
    public interface IBaseDbContext : IDisposable
    {


        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        //Database Database { get; }
        IBaseDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        bool AutoDetectChanges { get; set; }

        void AddEntity<TEntity>(TEntity entity) where TEntity : class;
        void AttachEntity<TEntity>(TEntity entity) where TEntity : class;
        void RemoveEntity<TEntity>(TEntity entity) where TEntity : class;
        TEntity FindEntity<TEntity>(object id) where TEntity : class;
        TEntity FindEntityLocal<TEntity>(object id) where TEntity : class, IBaseEntity;


        IQueryable<TEntity> Queryable<TEntity>() where TEntity : class;
        IQueryable<Object> Queryable(Type type);

        void UpdateEntity(object existingEntity, object newEntity);
        T UpdateGraph<T>(T entity, Expression<Func<IUpdateConfiguration<T>, object>> mapping = null) where T : class, new();
        Boolean IsEntityStateAdded(object entity);
        void SetEntityStateAdded(object entity);
        Boolean IsEntityStateDeleted(object entity);
        void SetEntityStateDeleted(object entity);
        Boolean IsEntityStateModified(object entity);
        void SetEntityStateModified(object entity);
        Boolean IsEntityStateDetached(object entity);
        void SetEntityStateDetached(object entity);
        Boolean IsEntityStateUnchanged(object entity);
        void SetEntityStateUnchanged(object entity);
        IEnumerable<DbEntityValidationResult> GetValidationErrors();

        IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class;
        IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class;
        Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class;
        Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class;
    }
}
