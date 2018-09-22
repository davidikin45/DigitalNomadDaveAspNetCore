using DND.Common.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LeacockSite.Data
{
    public abstract class ContextInitializerMigrate<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public ContextInitializerMigrate(
            TDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            var script = _context.GenerateMigrationScript();
            _context.Database.Migrate();

            Seed(_context);

            _context.SaveChanges();
        }

        public abstract void Seed(TDbContext context);
    }
}
