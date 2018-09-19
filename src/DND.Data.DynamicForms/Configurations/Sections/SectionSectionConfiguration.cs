using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Sections
{
    public class SectionSectionConfiguration
           : IEntityTypeConfiguration<SectionSection>
    {

        public void Configure(EntityTypeBuilder<SectionSection> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.HasOne(p => p.ChildSection)
                .WithMany()
                .HasForeignKey(p => p.ChildSectionId);
        }
    }
}
