using LeacockSite.Data;

namespace DND.Data.DynamicForms.Initializers
{
    public class DynamicFormsContextInitializerMigrate : ContextInitializerMigrate<DynamicFormsContext>
    {
        public DynamicFormsContextInitializerMigrate(
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
