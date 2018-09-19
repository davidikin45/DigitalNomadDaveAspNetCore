using DND.Data.Identity;
using Microsoft.EntityFrameworkCore;

namespace DND.UnitTests
{
    public class GlobalSetup
    {
        //Allows scope of Db to be controlled independently of the context.
        public static IdentityContext CreateIdentityContextInMemory(string dbIdentifier)
        {
            var db = new IdentityContext(new DbContextOptionsBuilder<IdentityContext>().UseInMemoryDatabase(databaseName: dbIdentifier).Options);
            return db;
        }
    }
}
