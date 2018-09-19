using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Sections
{
    public class SectionConfiguration
           : IEntityTypeConfiguration<Section>
    {
        public void Configure(EntityTypeBuilder<Section> builder)
        {
            builder.HasKey(p => p.Id);


            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.RowVersion).IsRowVersion();

            //this section has many sections, each section with a required owner
            builder.HasMany(p => p.Sections).WithOne().IsRequired().HasForeignKey(s => s.SectionId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
