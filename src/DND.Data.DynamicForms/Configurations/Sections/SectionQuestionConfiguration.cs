using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Sections
{
    public class SectionQuestionConfiguration
           : EntityTypeConfiguration<SectionQuestion>
    {
        public SectionQuestionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            HasRequired(p => p.Question)
                .WithMany()
                .HasForeignKey(p => p.QuestionId);
        }
    }
}
