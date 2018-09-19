using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.FormSectionSubmissions
{
    public class FormSectionSubmission : EntityAuditableBase<int>
    {
        public Guid FormSubmissionId { get; set; }

        public string UrlSlug { get; set; }

        public bool Valid { get; set; }

        public List<FormSectionSubmissionQuestionAnswer> QuestionAnswers { get; set; } = new List<FormSectionSubmissionQuestionAnswer>();

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public override async Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return await Task.FromResult(errors);
        }
    }
}
