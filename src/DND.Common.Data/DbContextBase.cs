using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Data
{

    public abstract class DbContextBase : DbContext
    {
        private DbContextTimestamps _dbContextTimestamps;

        public DbContextBase(DbContextOptions options)
            : base(options)
        {
            _dbContextTimestamps = new DbContextTimestamps();

            //context.Set<TEntity>()
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public static readonly ILoggerFactory LoggerFactory
        = new LoggerFactory()
          .AddDebug((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name))
          .AddConsole((categoryName, logLevel) => (logLevel == LogLevel.Information) && (categoryName == DbLoggerCategory.Database.Command.Name));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseLoggerFactory(LoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.HasDefaultSchema("dbo"); //SQLite doesnt have schemas

            builder.RemovePluralizingTableNameConvention();
        }

        #region Seed
        public abstract void Seed();
        #endregion

        #region Migrate
        public void Migrate()
        {
            Database.Migrate();
        }
        #endregion

        #region Timestamps
        private void AddTimestamps()
        {
            var added = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).Select(e => e.Entity);
            var modified = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).Select(e => e.Entity);
            var deleted = this.ChangeTracker.Entries().Where(e => e.State == EntityState.Deleted).Select(e => e.Entity);

            _dbContextTimestamps.AddTimestamps(added, modified, deleted);
        }
        #endregion

        #region Save Changes
        public new int SaveChanges()
        {
            int objectCount = 0;

            AddTimestamps();

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

            AddTimestamps();

            objectCount = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return objectCount;
        }
        #endregion
    }
}
