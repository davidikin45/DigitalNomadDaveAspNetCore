using DND.Domain.DynamicForms.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionConfiguration
           : IEntityTypeConfiguration<Question>
    {

        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.RowVersion).IsRowVersion();

            //NotMapped
            //Ignore(p => p.SectionType);

            builder.Property(p => p.QuestionTypeString)
                .HasColumnName(nameof(Question.QuestionType));

            builder.HasMany(p => p.Questions).WithOne().IsRequired().HasForeignKey(c => c.QuestionId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
