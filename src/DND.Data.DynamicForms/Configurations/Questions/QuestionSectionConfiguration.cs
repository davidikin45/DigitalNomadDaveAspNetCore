using DND.Domain.DynamicForms.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionSectionConfiguration
           : IEntityTypeConfiguration<QuestionSection>
    {
        public void Configure(EntityTypeBuilder<QuestionSection> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.HasOne(p => p.Section)
                .WithMany()
                .HasForeignKey(p => p.SectionId);
        }
    }
}
