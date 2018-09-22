using LeacockSite.Data;

namespace DND.Data.Identity.Initializers
{
    public class IdentityContextInitializerDropCreate : ContextInitializerDropCreate<IdentityContext>
    {
        public IdentityContextInitializerDropCreate(
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
