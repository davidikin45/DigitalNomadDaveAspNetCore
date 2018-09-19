using DND.Domain.DynamicForms.FormSubmissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.FormSubmissions
{
    public class FormSubmissionConfiguration
           : IEntityTypeConfiguration<FormSubmission>
    {

        public void Configure(EntityTypeBuilder<FormSubmission> builder)
        {
           builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);

            builder.Property(p => p.RowVersion).IsRowVersion();
        }
    }
}
