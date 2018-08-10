using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.Forms
{
    public class FormNotificationConfiguration
           : EntityTypeConfiguration<FormNotification>
    {
        public FormNotificationConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);
        }
    }
}
