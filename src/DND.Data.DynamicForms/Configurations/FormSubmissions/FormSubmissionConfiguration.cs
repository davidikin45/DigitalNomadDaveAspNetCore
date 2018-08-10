using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.FormSubmissions;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.FormSubmissions
{
    public class FormSubmissionConfiguration
           : EntityTypeConfiguration<FormSubmission>
    {
        public FormSubmissionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);

            Property(p => p.RowVersion).IsRowVersion();
                 
        }
    }
}
