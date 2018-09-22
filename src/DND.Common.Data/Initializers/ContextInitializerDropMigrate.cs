using DND.Common.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LeacockSite.Data
{
    public abstract class ContextInitializerDropMigrate<TDbContext>
         where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public ContextInitializerDropMigrate(
            TDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            //Delete database relating to this context only
            _context.EnsureDeleted();

            var script = _context.GenerateMigrationScript();
            _context.Database.Migrate();

            Seed(_context);

            _context.SaveChanges();
        }

        public abstract void Seed(TDbContext context);
    }
}
