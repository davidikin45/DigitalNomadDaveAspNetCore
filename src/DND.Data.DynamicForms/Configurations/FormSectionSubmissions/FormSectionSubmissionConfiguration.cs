using DND.Domain.DynamicForms.FormSectionSubmissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.FormSectionSubmissions
{
    public class FormSectionSubmissionConfiguration
           : IEntityTypeConfiguration<FormSectionSubmission>
    {
        public void Configure(EntityTypeBuilder<FormSectionSubmission> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);
        }
    }
}
