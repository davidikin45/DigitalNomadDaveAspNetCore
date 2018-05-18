using DND.EFPersistance.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.UnitTests
{
    public class GlobalSetup
    {
        //Allows scope of Db to be controlled independently of the context.
        public static ApplicationIdentityDbContext CreateIdentityContextInMemory(string dbIdentifier)
        {
            var db = new ApplicationIdentityDbContext(new DbContextOptionsBuilder<ApplicationIdentityDbContext>().UseInMemoryDatabase(databaseName: dbIdentifier).Options);
            return db;
        }
    }
}
