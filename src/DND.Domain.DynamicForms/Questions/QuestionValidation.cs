using DND.Common.Domain;
using DND.Common.Extensions;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.DynamicForms.Questions.Enums;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions
{
    public class QuestionValidation : EntityAuditableBase<int>
    {
        public int QuestionId { get; set; }

        public string ValidationTypeString
        {
            get { return ValidationType.ToString(); }
            private set { ValidationType = EnumExtensions.ParseEnum<QuestionValidationType>(value); }
        }

        public QuestionValidationType ValidationType = QuestionValidationType.Required;

        public string CustomValidationMessage { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(DbContext context, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return await Task.FromResult(errors);
        }
    }
}
