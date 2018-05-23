﻿using DND.Common.DomainEvents;
using DND.Common.Extensions;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using RefactorThis.GraphDiff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.SqlServer;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Persistance
{

    public abstract class BaseDbContext : DbContext, IBaseDbContext
    {
        private IDbContextDomainEvents _dbContextDomainEvents;

        public BaseDbContext(string nameOrConnectionString, bool logSql, IDomainEvents domainEvents = null)
        : base(nameOrConnectionString)
        {
            _dbContextDomainEvents = new DbContextDomainEventsEF6(this, domainEvents);

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
            _dbContextTimestamps = new DbContextTimestamps();

            SqlProviderServices.SqlServerTypesAssemblyName = "Microsoft.SqlServer.Types, Version=14.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            //UTC Date Time
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ReadAllDateTimeValuesAsUtc;

            //Change Tracking
            Configuration.AutoDetectChangesEnabled = false;

            Configuration.ValidateOnSaveEnabled = true;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
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

        private void AddTimestamps()
        {
            var added = _dbContextDomainEvents.GetNewInsertedEntities();
            var modified = _dbContextDomainEvents.GetNewUpdatedEntities();
            var deleted = _dbContextDomainEvents.GetNewDeletedEntities();

            _dbContextTimestamps.AddTimestamps(added, modified, deleted);
        }

        public new int SaveChanges()
        {
            return SaveChanges(false);
        }

        public int FireEvents()
        {
            return SaveChanges(true);
        }

        public int SaveChanges(bool preCommitOnly = false)
        {
            int objectCount = 0;

            AddTimestamps();

            _dbContextDomainEvents.FirePreCommitEventsAsync().Wait();

            if (!preCommitOnly)
            {
                objectCount = base.SaveChanges();
                _dbContextDomainEvents.FirePostCommitEventsAsync().Wait();
            }

            return objectCount;
        }

        public new async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(false).ConfigureAwait(false);
        }

        public async Task<int> FireEventsAsync()
        {
            return await SaveChangesAsync(true).ConfigureAwait(false);
        }

        public async Task<int> SaveChangesAsync(bool preCommitOnly = false)
        {
            return await SaveChangesAsync(CancellationToken.None, preCommitOnly).ConfigureAwait(false);
        }

        public new async Task<int> SaveChangesAsync(CancellationToken cancellationtoken)
        {
            return await SaveChangesAsync(cancellationtoken, false);
        }

        public  async Task<int> FireEventsAsync(CancellationToken cancellationtoken)
        {
            return await SaveChangesAsync(cancellationtoken, true);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken, bool preCommitOnly = false)
        {
            int objectCount = 0;

            AddTimestamps();

            await _dbContextDomainEvents.FirePreCommitEventsAsync().ConfigureAwait(false);

            if (!preCommitOnly)
            {
                objectCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await _dbContextDomainEvents.FirePostCommitEventsAsync().ConfigureAwait(false);
            }

            return objectCount;
        }

        IEnumerable<DbEntityValidationResultBetter> IBaseDbContext.GetValidationErrors()
        {
            return base.GetValidationErrors().Select(x => new DbEntityValidationResultBetter(x.ValidationErrors));
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

        public TEntity FindEntityLocal<TEntity>(object id) where TEntity : class, IBaseEntity
        {
            return Set<TEntity>().Local.Where(e => e.Id.Equals(id)).FirstOrDefault();
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
    }
}
