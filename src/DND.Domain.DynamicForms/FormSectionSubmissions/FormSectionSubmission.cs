using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.FormSectionSubmissions
{
    public class FormSectionSubmission : BaseEntityAuditable<int>
    {
        public Guid FormSubmissionId { get; set; }

        public bool Completed { get; set; }
        public bool Valid { get; set; }

        public List<FormSectionSubmissionQuestionAnswer> QuestionAnswers { get; set; } = new List<FormSectionSubmissionQuestionAnswer>();

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public override async Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
