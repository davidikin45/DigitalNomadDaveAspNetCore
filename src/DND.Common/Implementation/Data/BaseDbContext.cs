using DND.Common.DomainEvents;
using DND.Common.Extensions;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using RefactorThis.GraphDiff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Helpers;

namespace DND.Common.Implementation.Data
{
    public class EFTransactionInterceptor : IDbTransactionInterceptor
    {
        public void Committed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {
            foreach (var dbContext in interceptionContext.DbContexts)
            {
                if (dbContext is IBaseDbContext)
                {
                    ((IBaseDbContext)dbContext).FirePostCommitEvents();
                }
            }
        }

        public void Committing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void ConnectionGetting(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {

        }

        public void ConnectionGot(DbTransaction transaction, DbTransactionInterceptionContext<DbConnection> interceptionContext)
        {

        }

        public void Disposed(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void Disposing(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void IsolationLevelGetting(DbTransaction transaction, DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
        {

        }

        public void IsolationLevelGot(DbTransaction transaction, DbTransactionInterceptionContext<IsolationLevel> interceptionContext)
        {

        }

        public void RolledBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }

        public void RollingBack(DbTransaction transaction, DbTransactionInterceptionContext interceptionContext)
        {

        }
    }

    public abstract class BaseDbContext : DbContext, IBaseDbContext
    {
        private IDbContextDomainEvents _dbContextDomainEvents;

        public BaseDbContext(string nameOrConnectionString, bool logSql, IDomainEvents domainEvents = null)
        : base(nameOrConnectionString)
        {
            _dbContextDomainEvents = new DbContextDomainEventsEF6Adapter(this, domainEvents);

            if (logSql)
            {
                Database.Log = s => Debug.WriteLine(s);
            }

            init();

            //Once a migration is created DB is never created
            //Database.SetInitializer<BaseIdentityDbContext<T>>(new MigrateDatabaseToLatestVersion<BaseIdentityDbContext, T>());

            //Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseIfModelChanges<SchoolDBContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseAlways<SchoolDBContext>());
        }

        public BaseDbContext(DbConnection connection)
           : base(connection, true)
        {
            init();
        }

        private DbContextTimestamps _dbContextTimestamps;

        private void init()
        {
            DbInterception.Add(new EFTransactionInterceptor());

            _dbContextTimestamps = new DbContextTimestamps();

            SqlProviderServices.SqlServerTypesAssemblyName = "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            //Change Tracking
            Configuration.AutoDetectChangesEnabled = false;

            Configuration.ValidateOnSaveEnabled = true;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;

            //UTC Date Time
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ReadAllDateTimeValuesAsUtc;
        }

        public bool AutoDetectChanges
        {
            get { return this.Configuration.AutoDetectChangesEnabled; }
            set { this.Configuration.AutoDetectChangesEnabled = value; }
        }

        //public abstract static BaseIdentityDbContext Create(string nameOrConnectionString);
        //{
        //return new BaseIdentityDbContext(nameOrConnectionString);
        //}

        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public new DbSet Set(Type type)
        {
            return base.Set(type);
        }

        public void SetInitializer<TContext>(IDatabaseInitializer<TContext> initializer = null) where TContext : DbContext
        {
            //DbContextInitializer<TContext>.SetInitializer(initializer, true, false);
        }

        public int CachedEntityCount()
        {
            return this.ChangeTracker.Entries().Count();
        }

        public T UpdateGraph<T>(T entity, Expression<Func<IUpdateConfiguration<T>, object>> mapping = null) where T : class, new()
        {
            return DbContextExtensions.UpdateGraph(this, entity, mapping);
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

        public Boolean IsEntityStateAdded(object entity)
        {
            return Entry(entity).State == EntityState.Added;
        }

        public void SetEntityStateAdded(object entity)
        {
            Entry(entity).State = EntityState.Added;
        }

        public Boolean IsEntityStateDeleted(object entity)
        {
            return Entry(entity).State == EntityState.Deleted;
            
        }

        public void SetEntityStateDeleted(object entity)
        {
            Entry(entity).State = EntityState.Deleted;
        }

        public Boolean IsEntityStateModified(object entity)
        {
            return Entry(entity).State == EntityState.Modified;
        }

        public void SetEntityStateModified(object entity)
        {
            Entry(entity).State = EntityState.Modified;
        }

        public Boolean IsEntityStateDetached(object entity)
        {
            return Entry(entity).State == EntityState.Detached;
        }

        public void SetEntityStateDetached(object entity)
        {
            Entry(entity).State = EntityState.Detached;
        }

        public Boolean IsEntityStateUnchanged(object entity)
        {
            return Entry(entity).State == EntityState.Unchanged;
        }

        public void SetEntityStateUnchanged(object entity)
        {
            Entry(entity).State = EntityState.Unchanged;
        }

        public IBaseDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return new BaseDbContextTransaction(Database.BeginTransaction(isolationLevel));
        }

        public IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return base.Database.SqlQuery<TResultType>(query, paramaters).ToList();
        }

        public IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return base.Set<TResultType>().SqlQuery(query, paramaters).ToList();
        }

        public async Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return await base.Database.SqlQuery<TResultType>(query, paramaters).ToListAsync();
        }

        public async Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return await base.Set<TResultType>().SqlQuery(query, paramaters).ToListAsync();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Entity<IdentityUser>().ToTable("User");
            //modelBuilder.Entity<IdentityRole>().ToTable("Role");
            //modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            //modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            //modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
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

            AddTimestamps();

            objectCount = base.SaveChanges();

            return objectCount;
        }

        public new async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(CancellationToken.None).ConfigureAwait(false);
        }

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            int objectCount = 0;

            AddTimestamps();

            objectCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return objectCount;
        }

        IEnumerable<DbEntityValidationResultBetter> IBaseDbContext.GetValidationErrors()
        {
            return base.GetValidationErrors().Select(x => new DbEntityValidationResultBetter(x.ValidationErrors));
        }

        IEnumerable<DbEntityValidationResultBetter> IBaseDbContext.GetValidationErrorsForNewChanges()
        {
            return base.GetValidationErrors().Where(x => !_dbContextDomainEvents.GetPreCommittedDeletedEntities().Contains(x) && !_dbContextDomainEvents.GetPreCommittedInsertedEntities().Contains(x)).Select(x => new DbEntityValidationResultBetter(x.ValidationErrors));
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
            return (IQueryable<Object>)Set(type);
        }

        #region "UTC"
        private static void ReadAllDateTimeValuesAsUtc(object sender, ObjectMaterializedEventArgs evArg)
        {
            object entity = evArg.Entity;
            if (entity != null)
            {
                Type eType = entity.GetType();
                List<PropertyInfo> rules = (List<PropertyInfo>)PropsCache[eType];

                if (rules == null)
                    lock (PropsCache)
                        PropsCache[eType] = rules = GetDateProperties(eType); // Don't bother double-checking. Over-write is safe.

                foreach (var rule in rules)
                {
                    var info = rule;
                    object curVal = info.GetValue(entity);
                    if (curVal != null)
                        info.SetValue(entity, DateTime.SpecifyKind((DateTime)curVal, DateTimeKind.Utc));
                }
            }
        }

        private static readonly List<PropertyInfo> EmptyPropsList = new List<PropertyInfo>();
        private static readonly Hashtable PropsCache = new Hashtable(); // Spec promises safe for single-reader, multiple writer.
                                                                        // Spec for Dictionary makes no such promise, and while
                                                                        // it should be okay in this case, play it safe.
        private static List<PropertyInfo> GetDateProperties(Type type)
        {
            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                Type valType = prop.PropertyType;
                if (valType == typeof(DateTime) || valType == typeof(DateTime?))
                    list.Add(prop);
            }
            if (list.Count == 0)
                return EmptyPropsList; // Don't waste memory on lots of empty lists.
            list.TrimExcess();
            return list;
        }
        #endregion

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
