using DND.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace DND.UnitTests
{
    public class GlobalSetup
    {
        //Allows scope of Db to be controlled independently of the context.
        public static IdentityDbContext CreateIdentityContextInMemory(string dbIdentifier)
        {
            var db = new IdentityDbContext(new DbContextOptionsBuilder<IdentityDbContext>().UseInMemoryDatabase(databaseName: dbIdentifier).Options);
            return db;
        }
    }
}
