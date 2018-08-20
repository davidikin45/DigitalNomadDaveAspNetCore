using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Sections
{
    public class SectionSectionConfiguration
           : EntityTypeConfiguration<SectionSection>
    {
        public SectionSectionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            HasRequired(p => p.ChildSection)
                .WithMany()
                .HasForeignKey(p => p.ChildSectionId);
        }
    }
}
