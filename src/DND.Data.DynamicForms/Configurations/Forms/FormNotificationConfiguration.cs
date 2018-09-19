using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DND.Data.DynamicForms.Configurations.Forms
{
    public class FormNotificationConfiguration
           : IEntityTypeConfiguration<FormNotification>
    {

        public void Configure(EntityTypeBuilder<FormNotification> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Ignore(p => p.DateDeleted);
            builder.Ignore(p => p.UserDeleted);
        }
    }
}
