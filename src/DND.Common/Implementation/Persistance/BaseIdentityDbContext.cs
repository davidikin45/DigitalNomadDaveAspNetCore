using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RefactorThis.GraphDiff;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading;
using DND.Common.Extensions;
using Microsoft.AspNetCore.Identity;
using DND.Common.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using DND.Common.Implementation.Validation;

namespace DND.Common.Implementation.Persistance
{
    public class BaseIdentityDbContext<TUser> : IdentityDbContext<TUser>, IBaseDbContext where TUser : BaseApplicationUser
    {
        public BaseIdentityDbContext(DbContextOptions options)
            :base(options)
        {
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

        public bool AutoDetectChanges{
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

                    if(errors.Count() > 0)
                    {
                        var dbValidationErrors = new List<DbValidationError>();
                        foreach(ValidationResult error in errors)
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

        public override int SaveChanges()
        {
            AddTimestamps();

            var all = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));
            //maintain order for event firing
            var allEntities = all.Select(x => x.Entity);
            var inserted = all.Where(x => x.State == EntityState.Added).Select(x => x.Entity);
            var updated = all.Where(x => x.State == EntityState.Modified).Select(x => x.Entity);
            var deleted = all.Where(x => x.State == EntityState.Deleted).Select(x => x.Entity);

            BaseDbContext.RaiseDomainEventsPreCommit(allEntities, inserted, updated, deleted);

            var objectCount = base.SaveChanges();

            BaseDbContext.RaiseDomainEventsPostCommit(allEntities, inserted, updated, deleted);

            return objectCount;
        }

        public async Task<int> SaveChangesAsync()
        {
            AddTimestamps();

            var all = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));
            //maintain order for event firing
            var allEntities = all.Select(x => x.Entity);
            var inserted = all.Where(x => x.State == EntityState.Added).Select(x => x.Entity);
            var updated = all.Where(x => x.State == EntityState.Modified).Select(x => x.Entity);
            var deleted = all.Where(x => x.State == EntityState.Deleted).Select(x => x.Entity);

            BaseDbContext.RaiseDomainEventsPreCommit(allEntities, inserted, updated, deleted);

            var objectCount = await base.SaveChangesAsync();

            BaseDbContext.RaiseDomainEventsPostCommit(allEntities, inserted, updated, deleted);

            return objectCount;
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AddTimestamps();

            var all = ChangeTracker.Entries().Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified || x.State == EntityState.Deleted));
            //maintain order for event firing
            var allEntities = all.Select(x => x.Entity);
            var inserted = all.Where(x => x.State == EntityState.Added).Select(x => x.Entity);
            var updated = all.Where(x => x.State == EntityState.Modified).Select(x => x.Entity);
            var deleted = all.Where(x => x.State == EntityState.Deleted).Select(x => x.Entity);

            BaseDbContext.RaiseDomainEventsPreCommit(allEntities, inserted, updated, deleted);

            var objectCount = await base.SaveChangesAsync(cancellationToken);

            BaseDbContext.RaiseDomainEventsPostCommit(allEntities, inserted, updated, deleted);

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
    }
}
