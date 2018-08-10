using SQLite.CodeFirst;
using System.Data.Entity;

namespace DND.Data.Initializers
{
    //https://github.com/msallin/SQLiteCodeFirst
    //Will still pick up ApplicationDbConfiguration Migrations Config
    public class ApplicationDbInitializerDropCreateSQLite : SqliteDropCreateDatabaseAlways<ApplicationDbContext>
    {
        public ApplicationDbInitializerDropCreateSQLite(DbModelBuilder modelBuilder) 
            :base(modelBuilder)
        {

        }

        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            context.Seed();            
            base.Seed(context);
        }       
    }

}
