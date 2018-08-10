using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.FormSectionSubmissions
{
    public class FormSectionSubmissionConfiguration
           : EntityTypeConfiguration<FormSectionSubmission>
    {
        public FormSectionSubmissionConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);                 
        }
    }
}
