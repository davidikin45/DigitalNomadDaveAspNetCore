using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Sections
{
    public class SectionConfiguration
           : EntityTypeConfiguration<Section>
    {
        public SectionConfiguration()
        {
            HasKey(p => p.Id);


            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            //this section has many sections, each section with a required owner
            HasMany(p => p.Sections).WithRequired().HasForeignKey(s => s.SectionId).WillCascadeOnDelete(false);
        }
    }
}
