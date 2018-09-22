using LeacockSite.Data;

namespace DND.Data.Identity.Initializers
{
    public class IdentityContextInitializerMigrate : ContextInitializerMigrate<IdentityContext>
    {
        public IdentityContextInitializerMigrate(
            IdentityContext context)
            :base(context)
        {

        }

        public override void Seed(IdentityContext context)
        {
            context.Seed();
        }
    }
}
