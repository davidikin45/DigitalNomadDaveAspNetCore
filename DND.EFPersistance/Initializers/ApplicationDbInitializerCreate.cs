namespace DND.EFPersistance.Initializers
{
    //Will still pick up ApplicationDbConfiguration Migrations Config
    public class ApplicationDbInitializerCreate : System.Data.Entity.CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            DBSeed.Seed(context);            
            base.Seed(context);
        }       
    }

}
