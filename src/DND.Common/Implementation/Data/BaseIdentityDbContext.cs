using DND.Common.DomainEvents;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DiagnosticAdapter;
using Microsoft.Extensions.Logging;
using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Helpers;

namespace DND.Common.Implementation.Data
{
    public class TransactionListener
    {
        public event EventHandler TransactionCommitted;

        [DiagnosticName("Microsoft.EntityFrameworkCore.Database.Transaction.TransactionCommitted")]
        public void OnTransactionCommitted(IRelationalConnection connection, DbTransaction transaction, Guid transactionId, DateTimeOffset startTime, TimeSpan duration)
        {
            TransactionCommitted?.Invoke(this, new EventArgs());
        }
    }

    public class BaseIdentityDbContext<TUser> : IdentityDbContext<TUser>, IBaseDbContext where TUser : BaseApplicationUser
    {
        private IDbContextDomainEvents _dbContextDomainEvents;
        private DbContextTimestamps _dbContextTimestamps;

        public BaseIdentityDbContext(DbContextOptions options, IDomainEvents domainEvents = null)
            : base(options)
        {
            var listener = this.GetService<DiagnosticSource>();
            var transactionListener = new TransactionListener();
            transactionListener.TransactionCommitted += TransactionCommited;
            (listener as DiagnosticListener).SubscribeWithAdapter(transactionListener);

            _dbContextDomainEvents = new DbContextDomainEventsEFCoreAdapter(this, domainEvents);
            _dbContextTimestamps = new DbContextTimestamps();

            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        void TransactionCommited(object sender, EventArgs e)
        {
            FirePostCommitEvents();
        }

        public static readonly ILoggerFactory MyLoggerFactory
        = new LoggerFactory()
          .AddDebug((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name))
          .AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(MyLoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.HasDefaultSchema("dbo"); //SQLite doesnt have schemas

            builder.RemovePluralizingTableNameConvention();
            //modelBuilder.Entity<IdentityUser>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");

            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        }

        public bool AutoDetectChanges
        {
            get { return this.ChangeTracker.AutoDetectChangesEnabled; }
            set { this.ChangeTracker.AutoDetectChangesEnabled = value; }
        }

        public IBaseDbContextTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new BaseDbContextTransactionAspNetcore(Database.BeginTransaction());
        }

        //Validate on Save
        IEnumerable<DbEntityValidationResultBetter> IBaseDbContext.GetValidationErrors()
        {
            return GetValidationErrors(false);
        }

        IEnumerable<DbEntityValidationResultBetter> IBaseDbContext.GetValidationErrorsForNewChanges()
        {
            return GetValidationErrors(true);
        }

        IEnumerable<DbEntityValidationResultBetter> GetValidationErrors(bool onlyNewChanges)
        {
            var list = new List<DbEntityValidationResultBetter>();

            //var serviceProvider = this.GetService<IServiceProvider>();
            //var items = new Dictionary<object, object>();

            var entities = this.ChangeTracker.Entries().Where(e => ((e.State == EntityState.Added) || (e.State == EntityState.Modified)));
            if(onlyNewChanges)
            {
                entities = entities.Where(x => !_dbContextDomainEvents.GetPreCommittedDeletedEntities().Contains(x) && !_dbContextDomainEvents.GetPreCommittedInsertedEntities().Contains(x));
            }

            foreach (var entry in entities)
            {
                var entity = entry.Entity;
                //var context = new ValidationContext(entity, serviceProvider, items);
                //var results = new List<ValidationResult>();

                var results = ValidationHelper.ValidateObject(entity);


                if (results.Count() > 0)
                {
                    var errors = results.Where(r => r != ValidationResult.Success);

                    if (errors.Count() > 0)
                    {
                        var dbValidationErrors = new List<DbValidationError>();
                        foreach (ValidationResult error in errors)
                        {
                            if (error.MemberNames.Count() > 0)
                            {
                                foreach (var prop in error.MemberNames)
                                {
                                    dbValidationErrors.Add(new DbValidationError(prop, error.ErrorMessage));
                                }
                            }
                            else
                            {
                                dbValidationErrors.Add(new DbValidationError("", error.ErrorMessage));
                            }
                        }

                        var validationResult = new DbEntityValidationResultBetter(dbValidationErrors);

                        list.Add(validationResult);
                    }
                }
            }

            return list;
        }

        public int CachedEntityCount()
        {
            return this.ChangeTracker.Entries().Count();
        }

        public bool IsEntityStateAdded(object entity)
        {
            return Entry(entity).State == EntityState.Added;
        }

        public bool IsEntityStateDeleted(object entity)
        {
            return Entry(entity).State == EntityState.Deleted;
        }

        public bool IsEntityStateDetached(object entity)
        {
            return Entry(entity).State == EntityState.Detached;
        }

        public bool IsEntityStateModified(object entity)
        {
            return Entry(entity).State == EntityState.Modified;
        }

        public bool IsEntityStateUnchanged(object entity)
        {
            return Entry(entity).State == EntityState.Unchanged;
        }

        private void AddTimestamps()
        {
            var added = _dbContextDomainEvents.GetNewInsertedEntities();
            var modified = _dbContextDomainEvents.GetNewUpdatedEntities();
            var deleted = _dbContextDomainEvents.GetNewDeletedEntities();

            _dbContextTimestamps.AddTimestamps(added, modified, deleted);
       }

        public void FirePreCommitEvents()
        {
            FirePreCommitEventsAsync().Wait();
        }

        public async Task FirePreCommitEventsAsync()
        {
            AddTimestamps();

            await _dbContextDomainEvents.FirePreCommitEventsAsync().ConfigureAwait(false);
        }

        public void FirePostCommitEvents()
        {
            FirePostCommitEventsAsync().Wait();
        }

        public async Task FirePostCommitEventsAsync()
        {
            await _dbContextDomainEvents.FirePostCommitEventsAsync().ConfigureAwait(false);
        }

        public new int SaveChanges()
        {
            int objectCount = 0;

            FirePreCommitEvents();

            objectCount = base.SaveChanges();

            return objectCount;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(CancellationToken.None);
        }

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            int objectCount = 0;

            await FirePreCommitEventsAsync();

            objectCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return objectCount;
        }

        public void SetEntityStateAdded(object entity)
        {
            Entry(entity).State = EntityState.Added;
        }

        public void SetEntityStateDeleted(object entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public void SetEntityStateDetached(object entity)
        {
            Entry(entity).State = EntityState.Detached;
        }

        public void SetEntityStateModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public void SetEntityStateUnchanged(object entity)
        {
            Entry(entity).State = EntityState.Unchanged;
        }

        public void LoadCollectionProperty(object entity, string collectionProperty, int? skip = null, int? take = null)
        {
            var collectionItemType = entity.GetType().GetProperty(collectionProperty).PropertyType.GetGenericArguments().Single();

            Type iQueryableType = typeof(IQueryable<>).MakeGenericType(new[] { collectionItemType });

            var query = Entry(entity)
            .Collection(collectionProperty)
            .Query();

            if (skip.HasValue)
            {
                Expression<Func<int>> countAccessor = () => skip.Value;
                typeof(EntityFrameworkQueryableExtensions).GetMethod("Skip").MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query, countAccessor });
                //query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                Expression<Func<int>> countAccessor = () => take.Value;
                typeof(EntityFrameworkQueryableExtensions).GetMethod("Take").MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query, countAccessor });
                //query.Take(take.Value);
            }

            typeof(EntityFrameworkQueryableExtensions).GetMethod("Load").MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query });
        }

        public async Task LoadCollectionPropertyAsync(object entity, string collectionProperty, int? skip = null, int? take = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var collectionItemType = entity.GetType().GetProperty(collectionProperty).PropertyType.GetGenericArguments().Single();

            Type iQueryableType = typeof(IQueryable<>).MakeGenericType(new[] { collectionItemType });

            var query = Entry(entity)
            .Collection(collectionProperty)
            .Query();

            if (skip.HasValue)
            {
                Expression<Func<int>> countAccessor = () => skip.Value;
                typeof(EntityFrameworkQueryableExtensions).GetMethod("Skip").MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query, countAccessor });
                //query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                Expression<Func<int>> countAccessor = () => take.Value;
                typeof(EntityFrameworkQueryableExtensions).GetMethod("Take").MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query, countAccessor });
                //query.Take(take.Value);
            }

            await ((Task)(typeof(EntityFrameworkQueryableExtensions).GetMethod("LoadAsync", new Type[] { iQueryableType, typeof(CancellationToken) }).MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query, cancellationToken }))).ConfigureAwait(false);
        }

        public int CollectionPropertyCount(object entity, string collectionProperty)
        {
            var collectionItemType = entity.GetType().GetProperty(collectionProperty).PropertyType.GetGenericArguments().Single();

            Type iQueryableType = typeof(IQueryable<>).MakeGenericType(new[] { collectionItemType });

            var query = Entry(entity)
            .Collection(collectionProperty)
            .Query();

            return ((int)(typeof(EntityFrameworkQueryableExtensions).GetMethod("Count", new Type[] { iQueryableType }).MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query })));
        }

        public async Task<int> CollectionPropertyCountAsync(object entity, string collectionProperty, CancellationToken cancellationToken)
        {
            var collectionItemType = entity.GetType().GetProperty(collectionProperty).PropertyType.GetGenericArguments().Single();

            Type iQueryableType = typeof(IQueryable<>).MakeGenericType(new[] { collectionItemType });

            var query = Entry(entity)
            .Collection(collectionProperty)
            .Query();

            return await ((Task<int>)(typeof(EntityFrameworkQueryableExtensions).GetMethods().Where(m => m.Name == "CountAsync" && m.GetParameters().Length == 2 && m.GetParameters()[1].ParameterType == typeof(CancellationToken)).FirstOrDefault().MakeGenericMethod(collectionItemType).Invoke(null, new object[] { query, cancellationToken }))).ConfigureAwait(false);
        }

        public IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return base.Set<TResultType>().AsNoTracking().FromSql(query, paramaters).ToList();
        }

        public async Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return await base.Set<TResultType>().AsNoTracking().FromSql(query, paramaters).ToListAsync();
        }

        public IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return base.Set<TResultType>().FromSql(query, paramaters).ToList();
        }

        public async Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return await base.Set<TResultType>().FromSql(query, paramaters).ToListAsync();
        }

        //Using this should give performance improvement.
        //https://msdn.microsoft.com/en-us/library/jj592677(v=vs.113).aspx
        //Note that only properties that are set to different values when copied from the other object will be marked as modified.
        public void UpdateEntity(object existingEntity, object newEntity)
        {
            Entry(existingEntity).CurrentValues.SetValues(newEntity);
        }

        public void TriggerTrackChanges(object newEntity)
        {
            var currentValues = Entry(newEntity).CurrentValues.Clone();
            Entry(newEntity).CurrentValues.SetValues(Entry(newEntity).OriginalValues);
            Entry(newEntity).CurrentValues.SetValues(currentValues);
        }

        public T UpdateGraph<T>(T entity, Expression<Func<IUpdateConfiguration<T>, object>> mapping = null) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public void AddEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Add(entity);
        }

        public void AttachEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Attach(entity);
        }

        public void RemoveEntity<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Remove(entity);
        }

        public IQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        public IQueryable<Object> Queryable(Type type)
        {
            throw new NotImplementedException();
        }

        #region Local Entity Cache
        public bool EntityExistsLocal<TEntity>(TEntity entity) where TEntity : class
        {
            return Set<TEntity>().Local.Any(x => Equals(x, entity));
        }

        public bool EntityExistsByIdLocal<TEntity>(object id) where TEntity : class
        {
            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id).Compile();
                return Set<TEntity>().Local.Any(filter);
            }
            else
            {
                return false;
            }
        }

        public TEntity FindEntityByIdLocal<TEntity>(object id) where TEntity : class
        {
            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id).Compile();
                return Set<TEntity>().Local.Where(filter).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public TEntity FindEntityLocal<TEntity>(TEntity entity) where TEntity : class
        {
            return Set<TEntity>().Local.FirstOrDefault(x => Equals(x, entity));
        }
        #endregion

        #region Entity By Object
        public bool EntityExists<TEntity>(TEntity entity) where TEntity : class
        {
            var local = EntityExistsLocal(entity);
            if (local)
                return true;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return Set<TEntity>().Where(filter).ToList().Any();
            }

            return false;
        }

        public async Task<bool> EntityExistsAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var local = EntityExistsLocal(entity);
            if (local)
                return true;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return (await Set<TEntity>().Where(filter).ToListAsync(cancellationToken)).Any();
            }

            return false;
        }

        public bool EntityExistsNoTracking<TEntity>(TEntity entity) where TEntity : class
        {
            var local = EntityExistsLocal(entity);
            if (local)
                return true;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return Set<TEntity>().AsNoTracking().Where(filter).Any();
            }

            return false;
        }

        public async Task<bool> EntityExistsNoTrackingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var local = EntityExistsLocal(entity);
            if (local)
                return true;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return await Set<TEntity>().AsNoTracking().Where(filter).AnyAsync(cancellationToken);
            }

            return false;
        }

        public TEntity FindEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var local = FindEntityLocal<TEntity>(entity);
            if (local != null)
                return local;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return Set<TEntity>().Where(filter).SingleOrDefault();
            }

            return null;
        }

        public async Task<TEntity> FindEntityAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var local = FindEntityLocal<TEntity>(entity);
            if (local != null)
                return local;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return await Set<TEntity>().Where(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public TEntity FindEntityNoTracking<TEntity>(TEntity entity) where TEntity : class
        {
            var local = FindEntityLocal<TEntity>(entity);
            if (local != null)
                return local;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return Set<TEntity>().AsNoTracking().Where(filter).SingleOrDefault();
            }

            return null;
        }

        public async Task<TEntity> FindEntityNoTrackingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken) where TEntity : class
        {
            var local = FindEntityLocal<TEntity>(entity);
            if (local != null)
                return local;

            if (entity.HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), entity.GetPropValue(nameof(IBaseEntity.Id))))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(entity.GetPropValue(nameof(IBaseEntity.Id)));
                return await Set<TEntity>().AsNoTracking().Where(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }
        #endregion

        #region Entity By Id
        public bool EntityExistsById<TEntity>(object id) where TEntity : class
        {
            var local = EntityExistsByIdLocal<TEntity>(id);
            if (local)
                return true;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return Set<TEntity>().Where(filter).ToList().Any();
            }

            return false;
        }

        public async Task<bool> EntityExistsByIdAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var local = EntityExistsByIdLocal<TEntity>(id);
            if (local)
                return true;

            if (typeof(TEntity) is IBaseEntity && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return (await Set<TEntity>().Where(filter).ToListAsync(cancellationToken)).Any();
            }

            return false;
        }

        public bool EntityExistsByIdNoTracking<TEntity>(object id) where TEntity : class
        {
            var local = EntityExistsByIdLocal<TEntity>(id);
            if (local)
                return true;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return Set<TEntity>().AsNoTracking().Where(filter).Any();
            }

            return false;
        }

        public async Task<bool> EntityExistsByIdNoTrackingAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var local = EntityExistsByIdLocal<TEntity>(id);
            if (local)
                return true;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return await Set<TEntity>().AsNoTracking().Where(filter).AnyAsync(cancellationToken);
            }

            return false;
        }

        public TEntity FindEntityById<TEntity>(object id) where TEntity : class
        {
            //Will track
            return Set<TEntity>().Find(id);
        }

        public async Task<TEntity> FindEntityByIdAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var local = FindEntityByIdLocal<TEntity>(id);
            if (local != null)
                return local;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return await Set<TEntity>().Where(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }

        public TEntity FindEntityByIdNoTracking<TEntity>(object id) where TEntity : class
        {
            var local = FindEntityByIdLocal<TEntity>(id);
            if (local != null)
                return local;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return Set<TEntity>().AsNoTracking().Where(filter).SingleOrDefault();
            }

            return null;
        }

        public async Task<TEntity> FindEntityByIdNoTrackingAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class
        {
            var local = FindEntityByIdLocal<TEntity>(id);
            if (local != null)
                return local;

            if (typeof(TEntity).HasProperty(nameof(IBaseEntity.Id)) && !Equals(typeof(TEntity).GetProperty(nameof(IBaseEntity.Id)).PropertyType.DefaultValue(), id))
            {
                var filter = LamdaHelper.SearchForEntityById<TEntity>(id);
                return await Set<TEntity>().AsNoTracking().Where(filter).SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            }

            return null;
        }
        #endregion
    }
}
