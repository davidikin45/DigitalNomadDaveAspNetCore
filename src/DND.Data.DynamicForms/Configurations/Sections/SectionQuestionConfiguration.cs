using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Sections
{
    public class SectionQuestionConfiguration
           : IEntityTypeConfiguration<SectionQuestion>
    {
        public void Configure(EntityTypeBuilder<SectionQuestion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.HasOne(p => p.Question)
                .WithMany()
                .HasForeignKey(p => p.QuestionId);
        }
    }
}
