namespace DND.EFPersistance.Initializers
{
    //Will still pick up ApplicationDbConfiguration Migrations Config
    public class ApplicationDbInitializerDropCreateModelChange : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            DBSeed.Seed(context);            
            base.Seed(context);
        }       
    }
   
}
