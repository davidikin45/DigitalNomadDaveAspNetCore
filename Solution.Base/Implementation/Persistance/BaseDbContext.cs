using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Solution.Base.Interfaces.Models;

using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Solution.Base.Implementation.Models;
using System.Threading;
using Solution.Base.Interfaces.Persistance;
using System.Data.Entity.Infrastructure;
using RefactorThis.GraphDiff;
using System.Linq.Expressions;
using Solution.Base.Infrastructure;
using System.Reflection;
using System.Collections;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using Solution.Base.DomainEvents;

namespace Solution.Base.Implementation.Persistance
{
  
        public abstract class BaseDbContext : DbContext, IBaseDbContext
        {
            public BaseDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            SqlProviderServices.SqlServerTypesAssemblyName = "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            //UTC Date Time
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ReadAllDateTimeValuesAsUtc;

            //Change Tracking
            Configuration.AutoDetectChangesEnabled = false;

            Configuration.ValidateOnSaveEnabled = true;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;

            //Database.Log = Console.WriteLine;

            //Once a migration is created DB is never created
            //Database.SetInitializer<BaseIdentityDbContext<T>>(new MigrateDatabaseToLatestVersion<BaseIdentityDbContext, T>());

            //Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseIfModelChanges<SchoolDBContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseAlways<SchoolDBContext>());
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

        public T UpdateGraph<T>(T entity, Expression<Func<IUpdateConfiguration<T>, object>> mapping = null) where T : class, new()
        {
            return DbContextExtensions.UpdateGraph(this, entity, mapping);
        }

        public void UpdateEntity(object existingEntity, object newEntity)
        {
            Entry(existingEntity).CurrentValues.SetValues(newEntity);
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

        public override int SaveChanges()
        {
            AddTimestamps();

            var all = ChangeTracker.Entries<IBaseEntity>().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)).ToArray();
            var inserted  = all.Where(x => x.State == EntityState.Added).ToArray();
            var updated = all.Where(x => x.State == EntityState.Modified).ToArray();
            var deleted = all.Where(x => x.State == EntityState.Deleted).ToArray();

            RaiseDomainEventsPreCommit(all, inserted, updated, deleted);

            var objectCount = base.SaveChanges();

            RaiseDomainEventsPostCommit(all, inserted, updated, deleted);

            return objectCount;
        }

        public override async Task<int> SaveChangesAsync()
        {
            AddTimestamps();

            var all = ChangeTracker.Entries<IBaseEntity>().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)).ToArray();
            var inserted = all.Where(x => x.State == EntityState.Added).ToArray();
            var updated = all.Where(x => x.State == EntityState.Modified).ToArray();
            var deleted = all.Where(x => x.State == EntityState.Deleted).ToArray();

            RaiseDomainEventsPreCommit(all, inserted, updated, deleted);

            var objectCount = await base.SaveChangesAsync();

            RaiseDomainEventsPostCommit(all, inserted, updated, deleted);

            return objectCount;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationtoken)
        {
            AddTimestamps();

            var all = ChangeTracker.Entries<IBaseEntity>().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted)).ToArray();
            var inserted = all.Where(x => x.State == EntityState.Added).ToArray();
            var updated = all.Where(x => x.State == EntityState.Modified).ToArray();
            var deleted = all.Where(x => x.State == EntityState.Deleted).ToArray();

            RaiseDomainEventsPreCommit(all, inserted, updated, deleted);

            var objectCount = await base.SaveChangesAsync(cancellationtoken);

            RaiseDomainEventsPostCommit(all, inserted, updated, deleted);

            return objectCount;
        }

        private void RaiseDomainEventsPreCommit(
             DbEntityEntry<IBaseEntity>[] all,
            DbEntityEntry<IBaseEntity>[] inserted,
            DbEntityEntry<IBaseEntity>[] updated,
            DbEntityEntry<IBaseEntity>[] deleted)
        {
            foreach (var entityEntry in inserted)
            {
                Type genericType = typeof(InsertEntityEvent<>);
                Type[] typeArgs = { entityEntry.Entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entityEntry.Entity);
                DomainEvents.DomainEvents.DispatchPreCommit((IDomainEvent)domainEvent);
            }

            foreach (var entityEntry in updated)
            {
                Type genericType = typeof(UpdateEntityEvent<>);
                Type[] typeArgs = { entityEntry.Entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entityEntry.Entity);
                DomainEvents.DomainEvents.DispatchPreCommit((IDomainEvent)domainEvent);
            }

            foreach (var entityEntry in deleted)
            {
                Type genericType = typeof(DeleteEntityEvent<>);
                Type[] typeArgs = { entityEntry.Entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entityEntry.Entity);
                DomainEvents.DomainEvents.DispatchPreCommit((IDomainEvent)domainEvent);
            }
        }

        private void RaiseDomainEventsPostCommit(
            DbEntityEntry<IBaseEntity>[] all,
            DbEntityEntry<IBaseEntity>[] inserted,
            DbEntityEntry<IBaseEntity>[] updated,
            DbEntityEntry<IBaseEntity>[] deleted)
        {
            foreach (var entityEntry in inserted)
            {
                Type genericType = typeof(InsertEntityEvent<>);
                Type[] typeArgs = { entityEntry.Entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entityEntry.Entity);
                DomainEvents.DomainEvents.DispatchPostCommit((IDomainEvent)domainEvent);
            }

            foreach (var entityEntry in updated)
            {
                Type genericType = typeof(UpdateEntityEvent<>);
                Type[] typeArgs = { entityEntry.Entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entityEntry.Entity);
                DomainEvents.DomainEvents.DispatchPostCommit((IDomainEvent)domainEvent);
            }

            foreach (var entityEntry in deleted)
            {
                Type genericType = typeof(DeleteEntityEvent<>);
                Type[] typeArgs = { entityEntry.Entity.GetType() };
                Type constructed = genericType.MakeGenericType(typeArgs);
                object domainEvent = Activator.CreateInstance(constructed, entityEntry.Entity);
                DomainEvents.DomainEvents.DispatchPostCommit((IDomainEvent)domainEvent);
            }

            foreach (var entityEntry in all)
            {
                if (entityEntry.Entity is IBaseEntityAggregateRoot)
                {
                    var aggRootEntity = ((IBaseEntityAggregateRoot)entityEntry.Entity);
                    var events = aggRootEntity.DomainEvents.ToArray();
                    aggRootEntity.ClearEvents();
                    foreach (var domainEvent in events)
                    {
                        DomainEvents.DomainEvents.DispatchPostCommit(domainEvent);
                    }
                }
            }
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

        public TEntity FindEntity<TEntity>(object id) where TEntity : class
        {
            return Set<TEntity>().Find(id);
        }

        public IQueryable<TEntity> Queryable<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        public IQueryable<Object> Queryable(Type type)
        {
            return (IQueryable<Object>)Set(type);
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is IBaseEntityAuditable && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentUsername = !string.IsNullOrEmpty(Thread.CurrentPrincipal?.Identity?.Name)
              ? Thread.CurrentPrincipal.Identity.Name
                : "Anonymous";

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((IBaseEntityAuditable)entity.Entity).DateCreated = DateTime.UtcNow;
                    ((IBaseEntityAuditable)entity.Entity).UserCreated = currentUsername;
                }

                ((IBaseEntityAuditable)entity.Entity).DateModified = DateTime.UtcNow;
                ((IBaseEntityAuditable)entity.Entity).UserModified = currentUsername;
            }
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
    }
}
