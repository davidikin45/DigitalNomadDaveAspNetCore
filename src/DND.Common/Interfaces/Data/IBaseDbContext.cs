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
using DND.Common.Implementation.Validation;

namespace DND.Common.Interfaces.Data
{
    public interface IBaseDbContext : IDisposable
    {
        void FirePreCommitEvents();
        Task FirePreCommitEventsAsync();

        void FirePostCommitEvents();
        Task FirePostCommitEventsAsync();

        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        //Database Database { get; }
        IBaseDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        bool AutoDetectChanges { get; set; }

        void LoadCollectionProperty(object entity, string collectionExpression, string search = "", string orderBy = null, bool ascending = false, int? skip = null, int? take = null);
        Task LoadCollectionPropertyAsync(object entity, string collectionExpression, string search = "", string orderBy = null, bool ascending = false, int? skip = null, int? take = null, CancellationToken cancellationToken = default(CancellationToken));
        int  CollectionPropertyCount(object entity, string collectionExpression, string search = "");
        Task<int> CollectionPropertyCountAsync(object entity, string collectionExpression, string search, CancellationToken cancellationToken);

        void AddEntity<TEntity>(TEntity entity) where TEntity : class;
        void AttachEntity<TEntity>(TEntity entity) where TEntity : class;
        void RemoveEntity<TEntity>(TEntity entity) where TEntity : class;

        bool EntityExistsLocal<TEntity>(TEntity entity) where TEntity : class;
        bool EntityExistsByIdLocal<TEntity>(object id) where TEntity : class;
        TEntity FindEntityByIdLocal<TEntity>(object id) where TEntity : class;
        TEntity FindEntityLocal<TEntity>(TEntity entity) where TEntity : class;

        bool EntityExists<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> EntityExistsAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class;
        bool EntityExistsNoTracking<TEntity>(TEntity entity) where TEntity : class;
        Task<bool> EntityExistsNoTrackingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class;
        TEntity FindEntity<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> FindEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class;
        TEntity FindEntityNoTracking<TEntity>(TEntity entity) where TEntity : class;
        Task<TEntity> FindEntityNoTrackingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class;

        bool EntityExistsById<TEntity>(object id) where TEntity : class;
        Task<bool> EntityExistsByIdAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class;
        bool EntityExistsByIdNoTracking<TEntity>(object id) where TEntity : class;
        Task<bool> EntityExistsByIdNoTrackingAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class;
        TEntity FindEntityById<TEntity>(object id) where TEntity : class;
        Task<TEntity> FindEntityByIdAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class;
        TEntity FindEntityByIdNoTracking<TEntity>(object id) where TEntity : class;
        Task<TEntity> FindEntityByIdNoTrackingAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class;

        int CachedEntityCount();

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
        IEnumerable<DbEntityValidationResultBetter> GetValidationErrors();
        IEnumerable<DbEntityValidationResultBetter> GetValidationErrorsForNewChanges();

        void TriggerTrackChanges(object newEntity);

        IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class;
        IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class;
        Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class;
        Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class;
    }
}
