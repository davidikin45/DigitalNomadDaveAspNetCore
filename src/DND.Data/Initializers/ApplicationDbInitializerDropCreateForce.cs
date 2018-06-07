using DND.Common.Implementation.Data;

namespace DND.Data.Initializers
{
    //Will still pick up ApplicationDbConfiguration Migrations Config
    public class ApplicationDbInitializerDropCreateForce : BaseDropCreateForceDatabaseInitializer<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            context.Database.CommandTimeout = 180;
            DBSeed.Seed(context);            
            base.Seed(context);
        }       
    }
  
}
