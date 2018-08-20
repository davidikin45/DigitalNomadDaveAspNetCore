using DND.Domain.DynamicForms.Questions;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionConfiguration
           : EntityTypeConfiguration<Question>
    {
        public QuestionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();

            //NotMapped
            //Ignore(p => p.SectionType);

            Property(p => p.QuestionTypeString)
                .HasColumnName(nameof(Question.QuestionType));

            HasMany(p => p.Questions).WithRequired().HasForeignKey(c => c.QuestionId).WillCascadeOnDelete(false);
        }
    }
}
