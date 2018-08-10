using System.Data.Entity.Migrations;
using DND.Common.Implementation.Data;
using DND.Data.Initializers;

//enable-migrations -ContextProjectName DND.Data -StartUpProjectName WebApplication1 -ContextTypeName EFPersistance.ApplicationDbContext -ProjectName DND.Data
//Add-Migration InitialMigration
//Add-Migration "Name"
//Update-Database -- For Local Test Environment

//Update-database -TargetMigration:0 -Force
//Add-Migration InitialCreate -force

//hose articles are very clear so if you don't understand the difference it means that you didn't concentrate while reading the text and you also probably didn't follow the text by coding examples yourselves.

//Automatic migration is just a magic tool.You run your application and you will always get your database in the latest version because EF will do implicit migration every time it is needed - in the purest version you never need to do anything more than enabling automatic migrations.

//Automatic migrations are sometimes not enough. You need to add some customization to migration code or run some additional SQL commands for example to transform data. In such case you add explicit code based migration by calling Add-Migration command. Explicit migration shows all migration code which will be executed during migration (there is no additional magic).

//If you turn off automatic migrations you must always define explicit migration to define database upgrading process in well defined explicit steps.This is especially useful for scenarios where you need to use both upgrading and downgrading to specific version.

//Do not use Update-Database command to update your database in production, let the framework do it for you at startup using the MigrateDatabaseToLatestVersion initializer.
//
namespace DND.Data.Migrations
{
    //This applies to ALL Initializers
    public sealed class ApplicationDbMigrationConfigurationSQLite : BaseDbConfigurationSQLite<ApplicationDbContext>
    {
        public ApplicationDbMigrationConfigurationSQLite()
        {      
            //Seed(ApplicationDbContext.Create());


        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            context.Seed();
            base.Seed(context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
