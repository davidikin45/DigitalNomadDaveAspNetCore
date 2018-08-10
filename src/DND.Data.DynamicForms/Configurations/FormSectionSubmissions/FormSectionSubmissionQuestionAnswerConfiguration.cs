using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity.ModelConfiguration;

namespace DND.Data.DynamicForms.Configurations.FormSectionSubmissions
{
    public class FormSectionSubmissionQuestionAnswerConfiguration
           : EntityTypeConfiguration<FormSectionSubmissionQuestionAnswer>
    {
        public FormSectionSubmissionQuestionAnswerConfiguration()
        {
            HasKey(p => p.Id);

            Ignore(p => p.DateDeleted);
            Ignore(p => p.UserDeleted);
        }
    }
}
