using LeacockSite.Data;

namespace DND.Data.Initializers
{
    public class ApplicationContextInitializerMigrate : ContextInitializerMigrate<ApplicationContext>
    {
        public ApplicationContextInitializerMigrate(ApplicationContext context)
            :base(context)
        {

        }

        public override void Seed(ApplicationContext context)
        {
            context.Seed();
        }
    }
}
