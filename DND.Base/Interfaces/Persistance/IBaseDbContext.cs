using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using System.Data;

namespace DND.Base.Interfaces.Persistance
{
    public interface IBaseDbContext : IDisposable
    {

        void Initialize();
        int SaveChanges();
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        //Database Database { get; }
        IBaseDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);
        ChangeTracker ChangeTracker { get; }
        DbSet<T> Set<T>() where T : class;
        EntityEntry Entry(object entity);
        void UpdateEntity(object existingEntity, object newEntity);
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


        IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class;
        IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class;
        Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class;
        Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class;
    }
}
