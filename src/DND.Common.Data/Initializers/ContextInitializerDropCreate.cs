using DND.Common.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace LeacockSite.Data
{
    public abstract class ContextInitializerDropCreate<TDbContext>
        where TDbContext : DbContext
    {
        private readonly TDbContext _context;

        public ContextInitializerDropCreate(
            TDbContext context)
        {
            _context = context;
        }

        public void Initialize()
        {
            //Delete database relating to this context only
            _context.EnsureDeleted();

            //Recreate databases with the current data model. This is useful for development as no migrations are applied.
            _context.EnsureCreated();

            Seed(_context);

            _context.SaveChanges();
        }

        public abstract void Seed(TDbContext context);
    }
}
