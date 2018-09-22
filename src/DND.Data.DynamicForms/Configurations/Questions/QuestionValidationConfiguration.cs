using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Questions.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

            builder
           .Property(e => e.ValidationType)
           .HasConversion(new EnumToStringConverter<QuestionValidationType>());
        }
    }
}
