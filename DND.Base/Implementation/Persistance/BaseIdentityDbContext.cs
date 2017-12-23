using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Reflection;
using System.Collections;
using DND.Base.Extensions;
using DND.Base.Interfaces.Persistance;
using DND.Base.Interfaces.Model;

namespace DND.Base.Implementation.Persistance
{
    public abstract class BaseIdentityDbContext<TUser> : IdentityDbContext<TUser>, IBaseIdentityDbContext<TUser> where TUser : IdentityUser
    {
        public BaseIdentityDbContext(DbContextOptions option)
            : base(option)
        {
            //Change Tracking
            ChangeTracker.AutoDetectChangesEnabled = false;

       
            //UTC Date Time
            //https://docs.microsoft.com/en-us/aspnet/web-api/overview/formats-and-model-binding/json-and-xml-serialization
            //((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ReadAllDateTimeValuesAsUtc;

            //Database.Log = Console.WriteLine;

            //Once a migration is created DB is never created
            //Database.SetInitializer<BaseIdentityDbContext<T>>(new MigrateDatabaseToLatestVersion<BaseIdentityDbContext, T>());

            //Database.SetInitializer<ApplicationDbContext>(new CreateDatabaseIfNotExists<ApplicationDbContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseIfModelChanges<SchoolDBContext>());
            //Database.SetInitializer<SchoolDBContext>(new DropCreateDatabaseAlways<SchoolDBContext>());
        }


        //public abstract static BaseIdentityDbContext Create(string nameOrConnectionString);
        //{
        //return new BaseIdentityDbContext(nameOrConnectionString);
        //}

        public abstract void Initialize();

        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
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

        public IEnumerable<TResultType> SQLQueryTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return base.Set<TResultType>().FromSql(query, paramaters).ToList();
        }

        public IEnumerable<TResultType> SQLQueryNoTracking<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return base.Set<TResultType>().FromSql(query, paramaters).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<TResultType>> SQLQueryTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return await base.Set<TResultType>().FromSql(query, paramaters).ToListAsync();
        }

        public async Task<IEnumerable<TResultType>> SQLQueryNoTrackingAsync<TResultType>(string query, params object[] paramaters) where TResultType : class
        {
            return await base.Set<TResultType>().FromSql(query, paramaters).AsNoTracking().ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");
            modelBuilder.RemovePluralizingTableNameConvention();

            modelBuilder.Entity<TUser>().ToTable("User");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
            modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        }

        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationtoken)
        {
            AddTimestamps();
            return await base.SaveChangesAsync(cancellationtoken);
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
        //private static void ReadAllDateTimeValuesAsUtc(object sender, ObjectMaterializedEventArgs evArg)
        //{
        //    object entity = evArg.Entity;
        //    if (entity != null)
        //    {
        //        Type eType = entity.GetType();
        //        List<PropertyInfo> rules = (List<PropertyInfo>)PropsCache[eType];

        //        if (rules == null)
        //            lock (PropsCache)
        //                PropsCache[eType] = rules = GetDateProperties(eType); // Don't bother double-checking. Over-write is safe.

        //        foreach (var rule in rules)
        //        {
        //            var info = rule;
        //            object curVal = info.GetValue(entity);
        //            if (curVal != null)
        //                info.SetValue(entity, DateTime.SpecifyKind((DateTime)curVal, DateTimeKind.Utc));
        //        }
        //    }
        //}

        //private static readonly List<PropertyInfo> EmptyPropsList = new List<PropertyInfo>();
        //private static readonly Hashtable PropsCache = new Hashtable(); // Spec promises safe for single-reader, multiple writer.
        //                                                                // Spec for Dictionary makes no such promise, and while
        //                                                                // it should be okay in this case, play it safe.
        //private static List<PropertyInfo> GetDateProperties(Type type)
        //{
        //    List<PropertyInfo> list = new List<PropertyInfo>();
        //    foreach (PropertyInfo prop in type.GetProperties())
        //    {
        //        Type valType = prop.PropertyType;
        //        if (valType == typeof(DateTime) || valType == typeof(DateTime?))
        //            list.Add(prop);
        //    }
        //    if (list.Count == 0)
        //        return EmptyPropsList; // Don't waste memory on lots of empty lists.
        //    list.TrimExcess();
        //    return list;
        //}
        #endregion
    }
}