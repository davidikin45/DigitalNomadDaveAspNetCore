using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Forms
{
    public class FormSectionConfiguration
           : IEntityTypeConfiguration<FormSection>
    {
        public void Configure(EntityTypeBuilder<FormSection> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);
        }
    }
}
