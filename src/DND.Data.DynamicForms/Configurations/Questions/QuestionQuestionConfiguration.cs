using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionQuestionConfiguration
           : EntityTypeConfiguration<QuestionQuestion>
    {
        public QuestionQuestionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            HasRequired(p => p.LogicQuestion)
                .WithMany()
                .HasForeignKey(p => p.LogicQuestionId);
        }
    }
}
