using DND.Common.Data;
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
using Microsoft.EntityFrameworkCore;

namespace DND.Data.DynamicForms
{
    //Set Context Project as Visual Studio StartUp Project
    //Add-Migration InitialMigration -Context DynamicFormsContext
    //Add-Migration "Name" -Context DynamicFormsContext
    //Update-Database
    //Remove-Migration
    public class DynamicFormsContext : DbContextBase
    {
        //Admin
        public DbSet<Form> Forms { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<LookupTable> LookupTables { get; set; }

        //Form Submissions
        public DbSet<FormSubmission> FormSubmissions { get; set; }
        public DbSet<FormSectionSubmission> FormSectionSubmissions { get; set; }

        public DynamicFormsContext(DbContextOptions options)
             : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new FormConfiguration());
            builder.ApplyConfiguration(new FormSectionConfiguration());
            builder.ApplyConfiguration(new FormNotificationConfiguration());
            builder.ApplyConfiguration(new SectionConfiguration());
            builder.ApplyConfiguration(new SectionSectionConfiguration());
            builder.ApplyConfiguration(new SectionQuestionConfiguration());
            builder.ApplyConfiguration(new QuestionConfiguration());
            builder.ApplyConfiguration(new QuestionQuestionConfiguration());
            builder.ApplyConfiguration(new QuestionSectionConfiguration());
            builder.ApplyConfiguration(new QuestionValidationConfiguration());
            builder.ApplyConfiguration(new LookupTableConfiguration());
            builder.ApplyConfiguration(new LookupTableItemConfiguration());

            builder.ApplyConfiguration(new FormSubmissionConfiguration());
            builder.ApplyConfiguration(new FormSectionSubmissionConfiguration());
            builder.ApplyConfiguration(new FormSectionSubmissionQuestionAnswerConfiguration());
        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}
