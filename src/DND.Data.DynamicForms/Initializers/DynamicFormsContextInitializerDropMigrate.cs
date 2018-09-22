using LeacockSite.Data;

namespace DND.Data.DynamicForms.Initializers
{
    public class DynamicFormsContextInitializerDropMigrate : ContextInitializerDropMigrate<DynamicFormsContext>
    {
        public DynamicFormsContextInitializerDropMigrate(
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
