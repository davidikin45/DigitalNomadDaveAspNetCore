using DND.Common.Interfaces.Data;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Domain.DynamicForms.FormSubmissions;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Sections;
using System.Data.Entity;

namespace DND.Interfaces.DynamicForms.Data
{
    public interface IDynamicFormsDbContext : IBaseDbContext
    {
        //Admin
        IDbSet<Form> Forms { get; set; }
        IDbSet<Section> Sections { get; set; }
        IDbSet<Question> Questions { get; set; }
        IDbSet<LookupTable> LookupTables { get; set; }

        //Form Submissions
        IDbSet<FormSubmission> FormSubmissions { get; set; }
        IDbSet<FormSectionSubmission> FormSectionSubmissions { get; set; }
    }
}
