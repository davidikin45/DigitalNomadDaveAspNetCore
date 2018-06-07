using System.Data.Entity.Migrations;
using System.Data.Entity;
using System.Data.SQLite.EF6.Migrations;
//enable-migrations -ContextProjectName DND.Data -StartUpProjectName WebApplication1 -ContextTypeName EFPersistance.ApplicationDbContext -ProjectName DND.Data
//Add-Migration "Name"
//Update-Database -- For Local Test Environment


namespace DND.Common.Implementation.Data
{ 
    public abstract class BaseDbConfigurationSQLite<ApplicationDbContext> : BaseDbConfiguration<ApplicationDbContext>
        where ApplicationDbContext : DbContext
    {
        public BaseDbConfigurationSQLite()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
            
        }
    }
}
