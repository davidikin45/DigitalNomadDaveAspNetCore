using DND.Domain.DynamicForms.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionQuestionConfiguration
           : IEntityTypeConfiguration<QuestionQuestion>
    {
        public void Configure(EntityTypeBuilder<QuestionQuestion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.HasOne(p => p.LogicQuestion)
                .WithMany()
                .HasForeignKey(p => p.LogicQuestionId);
        }
    }
}
