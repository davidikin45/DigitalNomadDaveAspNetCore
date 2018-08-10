using DND.Domain.DynamicForms.Forms;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Forms
{
    public class FormConfiguration
           : EntityTypeConfiguration<Form>
    {
        public FormConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();
                 
        }
    }
}
