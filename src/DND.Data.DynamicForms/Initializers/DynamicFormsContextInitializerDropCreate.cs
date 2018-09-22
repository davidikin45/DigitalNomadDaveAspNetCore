using LeacockSite.Data;

namespace DND.Data.DynamicForms.Initializers
{
    public class DynamicFormsContextInitializerDropCreate : ContextInitializerDropCreate<DynamicFormsContext>
    {
        public DynamicFormsContextInitializerDropCreate(
            DynamicFormsContext context)
            :base(context)
        {

        }

        public override void Seed(DynamicFormsContext context)
        {
            context.Seed();
        }
    }
}
