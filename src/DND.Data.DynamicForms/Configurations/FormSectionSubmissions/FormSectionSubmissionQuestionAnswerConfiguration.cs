using DND.Domain.DynamicForms.FormSectionSubmissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.FormSectionSubmissions
{
    public class FormSectionSubmissionQuestionAnswerConfiguration
           : IEntityTypeConfiguration<FormSectionSubmissionQuestionAnswer>
    {
        public void Configure(EntityTypeBuilder<FormSectionSubmissionQuestionAnswer> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);
        }
    }
}
