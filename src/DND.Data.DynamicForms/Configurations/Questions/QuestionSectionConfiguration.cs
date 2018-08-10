using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionSectionConfiguration
           : EntityTypeConfiguration<QuestionSection>
    {
        public QuestionSectionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            HasRequired(p => p.Section)
                .WithMany()
                .HasForeignKey(p => p.SectionId);
        }
    }
}
