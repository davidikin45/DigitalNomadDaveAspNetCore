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

            //NotMapped
            //Ignore(p => p.SectionType);

            Property(p => p.SectionTypeString)
                .HasColumnName(nameof(Section.SectionType));
                 
        }
    }
}
