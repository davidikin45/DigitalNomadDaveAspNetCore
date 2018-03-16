using SQLite.CodeFirst;
using System.Data.Entity;

namespace DND.EFPersistance.Initializers
{
    //https://github.com/msallin/SQLiteCodeFirst
    //Will still pick up ApplicationDbConfiguration Migrations Config
    public class ApplicationDbInitializerCreateSQLite : SqliteCreateDatabaseIfNotExists<ApplicationDbContext>
    {
        public ApplicationDbInitializerCreateSQLite(DbModelBuilder modelBuilder) 
            :base(modelBuilder)
        {

        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            DBSeed.Seed(context);            
            base.Seed(context);
        }       
    }

}
