using DND.Domain.DynamicForms.Questions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Questions
{
    public class QuestionValidationConfiguration
           : IEntityTypeConfiguration<QuestionValidation>
    {

        public void Configure(EntityTypeBuilder<QuestionValidation> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.ValidationTypeString)
             .HasColumnName(nameof(QuestionValidation.ValidationType));
        }
    }
}
