using DND.Common.DomainEvents;
using DND.Common.Implementation.Data;
using DND.Data.DynamicForms.Configurations.Forms;
using DND.Data.DynamicForms.Configurations.FormSectionSubmissions;
using DND.Data.DynamicForms.Configurations.FormSubmissions;
using DND.Data.DynamicForms.Configurations.LookupTables;
using DND.Data.DynamicForms.Configurations.Questions;
using DND.Data.DynamicForms.Configurations.Sections;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using DND.Domain.DynamicForms.FormSubmissions;
using DND.Domain.DynamicForms.LookupTables;
using DND.Domain.DynamicForms.Questions;
using DND.Domain.DynamicForms.Sections;
using DND.Interfaces.DynamicForms.Data;
using System.Data.Common;
using System.Data.Entity;

namespace DND.Data.DynamicForms
{
    [DbConfigurationType(typeof(DbConfigurationSwitcher))]
    public class DynamicFormsDbContext : BaseDbContext, IDynamicFormsDbContext
    {
        //Admin
        public IDbSet<Form> Forms { get; set; }
        public IDbSet<Section> Sections { get; set; }
        public IDbSet<Question> Questions { get; set; }
        public IDbSet<LookupTable> LookupTables { get; set; }

        //Form Submissions
        public IDbSet<FormSubmission> FormSubmissions { get; set; }
        public IDbSet<FormSectionSubmission> FormSectionSubmissions { get; set; }
      
        public DynamicFormsDbContext(string connectionString, bool logSql, IDomainEvents domainEvents = null)
            : base(connectionString, logSql, domainEvents)
        {
            this.Database.CommandTimeout = 180;
        }

        public DynamicFormsDbContext(DbConnection connection)
           : base(connection)
        {

            this.Database.CommandTimeout = 180;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new FormConfiguration());
            modelBuilder.Configurations.Add(new FormSectionConfiguration());
            modelBuilder.Configurations.Add(new FormNotificationConfiguration());
            modelBuilder.Configurations.Add(new SectionConfiguration());
            modelBuilder.Configurations.Add(new SectionQuestionConfiguration());
            modelBuilder.Configurations.Add(new QuestionConfiguration());
            modelBuilder.Configurations.Add(new QuestionQuestionConfiguration());
            modelBuilder.Configurations.Add(new QuestionSectionConfiguration());
            modelBuilder.Configurations.Add(new QuestionValidationConfiguration());
            modelBuilder.Configurations.Add(new LookupTableConfiguration());
            modelBuilder.Configurations.Add(new LookupTableItemConfiguration());

            modelBuilder.Configurations.Add(new FormSubmissionConfiguration());
            modelBuilder.Configurations.Add(new FormSectionSubmissionConfiguration());
            modelBuilder.Configurations.Add(new FormSectionSubmissionQuestionAnswerConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}
