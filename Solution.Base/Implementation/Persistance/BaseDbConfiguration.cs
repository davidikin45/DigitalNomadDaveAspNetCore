using System.Data.Entity.Migrations;
using System.Data.Entity;
//enable-migrations -ContextProjectName EFPersistance -StartUpProjectName WebApplication1 -ContextTypeName EFPersistance.ApplicationDbContext -ProjectName EFPersistance
//Add-Migration "Name"
//Update-Database -- For Local Test Environment


namespace Solution.Base.Implementation.Persistance
{ 
    public abstract class BaseDbConfiguration<ApplicationDbContext> : DbMigrationsConfiguration<ApplicationDbContext>
        where ApplicationDbContext : DbContext
    {
        public BaseDbConfiguration()
        {
            AutomaticMigrationsEnabled = true; //Automatic Migrations
            AutomaticMigrationDataLossAllowed = true;
            CommandTimeout = 300;
            MigrationsDirectory = @"Migrations"; //Code First Migrations
        }

        protected override void Seed(ApplicationDbContext context)
        {
            base.Seed(context);
        }
    }
}
