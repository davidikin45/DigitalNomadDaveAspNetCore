using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionValidationConfiguration
           : EntityTypeConfiguration<QuestionValidation>
    {
        public QuestionValidationConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.ValidationTypeString)
             .HasColumnName(nameof(QuestionValidation.ValidationType));
        }
    }
}
