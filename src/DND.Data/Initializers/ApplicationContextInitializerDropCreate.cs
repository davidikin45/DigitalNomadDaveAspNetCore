using LeacockSite.Data;

namespace DND.Data.Initializers
{
    public class ApplicationContextInitializerDropCreate : ContextInitializerDropCreate<ApplicationContext>
    {
        public ApplicationContextInitializerDropCreate(ApplicationContext context)
            :base(context)
        {

        }

        public override void Seed(ApplicationContext context)
        {
            context.Seed();
        }
    }
}
