using DND.Common.DomainEvents;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using RefactorThis.GraphDiff;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Persistance
{
    public class BaseIdentityDbContext<TUser> : IdentityDbContext<TUser>, IBaseDbContext where TUser : BaseApplicationUser
    {
        private IDbContextDomainEvents _dbContextDomainEvents;
        private DbContextTimestamps _dbContextTimestamps;

        public BaseIdentityDbContext(DbContextOptions options, IDbContextDomainEvents dbContextDomainEvents = null)
            : base(options)
        {
            _dbContextDomainEvents = dbContextDomainEvents;
            _dbContextTimestamps = new DbContextTimestamps();

            ChangeTracker.AutoDetectChangesEnabled = false;
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
            var list = new List<DbEntityValidationResultBetter>();

            //var serviceProvider = this.GetService<IServiceProvider>();
            //var items = new Dictionary<object, object>();

            foreach (var entry in this.ChangeTracker.Entries().Where(e => (e.State == EntityState.Added) || (e.State == EntityState.Modified)))
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

        #region Events
        private Dictionary<object, List<IDomainEvent>> precommitedUpdatedEvents = new Dictionary<object, List<IDomainEvent>>();
        private List<object> precommitedUpdatedEntities = new List<object>();
        private Dictionary<object, List<IDomainEvent>> precommitedPropertyUpdateEvents = new Dictionary<object, List<IDomainEvent>>();
        private Dictionary<object, List<IDomainEvent>> precommitedDeletedEvents = new Dictionary<object, List<IDomainEvent>>();
        private List<object> precommitedDeletedEntities = new List<object>();
        private Dictionary<object, List<IDomainEvent>> precommitedInsertedEvents = new Dictionary<object, List<IDomainEvent>>();
        private List<object> precommitedInsertedEntities = new List<object>();
        private Dictionary<object, List<IDomainEvent>> precommitedDomainEvents = new Dictionary<object, List<IDomainEvent>>();

        private async Task FirePreCommitEventsAsync()
        {
            var updatedEvents = GetNewUpdatedEvents();
            precommitedUpdatedEntities.AddRange(updatedEvents.Keys);
            precommitedUpdatedEvents = precommitedUpdatedEvents.Concat(updatedEvents).ToDictionary(x => x.Key, x => x.Value);

            var propertiesUpdatedEvents = GetNewPropertyUpdatedEvents();
            foreach (var entity in propertiesUpdatedEvents)
            {
                if (!precommitedPropertyUpdateEvents.ContainsKey(entity.Key))
                {
                    precommitedPropertyUpdateEvents.Add(entity.Key, new List<IDomainEvent>());
                }

                foreach (var ev in entity.Value)
                {
                    precommitedPropertyUpdateEvents[entity.Key].Add(ev);
                }
            }

            var deletedEvents = GetNewDeletedEvents();
            precommitedDeletedEntities.AddRange(deletedEvents.Keys);
            precommitedDeletedEvents = precommitedDeletedEvents.Concat(deletedEvents).ToDictionary(x => x.Key, x => x.Value);

            var insertedEvents = GetNewInsertedEvents();
            precommitedInsertedEntities.AddRange(insertedEvents.Keys);
            precommitedInsertedEvents = precommitedInsertedEvents.Concat(insertedEvents).ToDictionary(x => x.Key, x => x.Value);

            var domainEvents = GetNewDomainEvents();
            foreach (var entity in domainEvents)
            {
                if (!precommitedDomainEvents.ContainsKey(entity.Key))
                {
                    precommitedDomainEvents.Add(entity.Key, new List<IDomainEvent>());
                }

                foreach (var ev in entity.Value)
                {
                    precommitedDomainEvents[entity.Key].Add(ev);
                }
            }

            if (_dbContextDomainEvents != null)
            {
                await _dbContextDomainEvents.DispatchDomainEventsPreCommitAsync(updatedEvents, propertiesUpdatedEvents, deletedEvents, insertedEvents, domainEvents).ConfigureAwait(false);
            }
        }

        private async Task FirePostCommitEventsAsync()
        {
            if (_dbContextDomainEvents != null)
            {
                await _dbContextDomainEvents.DispatchDomainEventsPostCommitAsync(precommitedUpdatedEvents, precommitedPropertyUpdateEvents, precommitedDeletedEvents, precommitedInsertedEvents, precommitedDomainEvents).ConfigureAwait(false);
            }
        }

        public Dictionary<object, List<IDomainEvent>> GetNewDeletedEvents()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Deleted && !precommitedDeletedEntities.Contains(x.Entity)));
            var events = _dbContextDomainEvents?.CreateEntityDeletedEvents(entries.Select(x => x.Entity));

            if (events == null)
            {
                events = new Dictionary<object, List<IDomainEvent>>();
            }

            return events;
        }

        public IEnumerable<object> GetNewDeletedEntities()
        {
            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Deleted && !precommitedDeletedEntities.Contains(x.Entity)).Select(x => x.Entity);
            return entities;
        }

        public Dictionary<object, List<IDomainEvent>> GetNewInsertedEvents()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added && !precommitedInsertedEntities.Contains(x.Entity)));
            var events = _dbContextDomainEvents?.CreateEntityInsertedEvents(entries.Select(x => x.Entity));

            if (events == null)
            {
                events = new Dictionary<object, List<IDomainEvent>>();
            }

            return events;
        }

        public IEnumerable<object> GetNewInsertedEntities()
        {
            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Added && !precommitedInsertedEntities.Contains(x.Entity)).Select(x => x.Entity);
            return entities;
        }

        public Dictionary<object, List<IDomainEvent>> GetNewUpdatedEvents()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Modified && !precommitedUpdatedEntities.Contains(x.Entity)));
            var events = _dbContextDomainEvents?.CreateEntityUpdatedEvents(entries.Select(x => x.Entity));

            if (events == null)
            {
                events = new Dictionary<object, List<IDomainEvent>>();
            }

            return events;
        }

        public IEnumerable<object> GetNewUpdatedEntities()
        {
            var entities = ChangeTracker.Entries().Where(x => x.State == EntityState.Modified && !precommitedUpdatedEntities.Contains(x.Entity)).Select(x => x.Entity);
            return entities;
        }

        public Dictionary<object, List<IDomainEvent>> GetNewPropertyUpdatedEvents()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Modified));
            var events = _dbContextDomainEvents?.CreatePropertyUpdateEventsEFCore(entries);

            if (events == null)
            {
                events = new Dictionary<object, List<IDomainEvent>>();
            }

            var allDomainEvents = precommitedPropertyUpdateEvents.Values.MergeLists();

            foreach (var entity in events)
            {
                entity.Value.RemoveAll(e => allDomainEvents.Contains(e));
            }

            return events;
        }

        public Dictionary<object, List<IDomainEvent>> GetNewDomainEvents()
        {
            var entries = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));

            var events = _dbContextDomainEvents?.CreateEntityDomainEvents(entries.Select(x => x.Entity));

            if (events == null)
            {
                events = new Dictionary<object, List<IDomainEvent>>();
            }

            return events;
        }

        private void AddTimestamps()
        {
            var added = GetNewInsertedEntities();
            var modified = GetNewUpdatedEntities();
            var deleted = GetNewDeletedEntities();

            _dbContextTimestamps.AddTimestamps(added, modified, deleted);
        }
        #endregion

        public new int SaveChanges()
        {
            return SaveChanges(false);
        }

        public int FireEvents()
        {
            return SaveChanges(true);
        }

        public new int SaveChanges(bool preCommitOnly = false)
        {
            int objectCount = 0;

            AddTimestamps();

            FirePreCommitEventsAsync().Wait();

            if (!preCommitOnly)
            {
                objectCount = base.SaveChanges();
                FirePostCommitEventsAsync().Wait();
            }

            return objectCount;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await SaveChangesAsync(false);
        }

        public async Task<int> FireEventsAsync()
        {
            return await SaveChangesAsync(true);
        }

        public async Task<int> SaveChangesAsync(bool preCommitOnly = false)
        {
            return await SaveChangesAsync(CancellationToken.None);
        }

        public async new Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(cancellationToken, false);
        }

        public async Task<int> FireEventsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await SaveChangesAsync(cancellationToken, true);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken), bool preCommitOnly = false)
        {
            int objectCount = 0;

            AddTimestamps();

            await FirePreCommitEventsAsync().ConfigureAwait(false);

            if (!preCommitOnly)
            {
                objectCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                await FirePostCommitEventsAsync().ConfigureAwait(false);
            }

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

        public void UpdateEntity(object existingEntity, object newEntity)
        {
            Entry(existingEntity).CurrentValues.SetValues(newEntity);
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
            throw new NotImplementedException();
        }
    }
}
