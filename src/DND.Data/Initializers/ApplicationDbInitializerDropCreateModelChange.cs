namespace DND.Data.Initializers
{
    //Will still pick up ApplicationDbConfiguration Migrations Config
    public class ApplicationDbInitializerDropCreateModelChange : System.Data.Entity.DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            context.Seed();            
            base.Seed(context);
        }       
    }
   
}
