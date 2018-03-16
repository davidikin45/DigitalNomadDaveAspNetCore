
using DND.EFPersistance.Migrations;
using System.Data.Entity;

namespace DND.EFPersistance.Initializers
{

    //https://blog.oneunicorn.com/2013/05/28/database-initializer-and-migrations-seed-methods/
    public class ApplicationDbInitializerMigrateSQLite : MigrateDatabaseToLatestVersion<ApplicationDbContext, ApplicationDbMigrationConfigurationSQLite>
    {
       
    }
   
}
