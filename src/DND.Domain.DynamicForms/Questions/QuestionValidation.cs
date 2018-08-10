using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections;
using DND.Domain.DynamicForms.Sections.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions
{
    public class QuestionValidation : BaseEntityAuditable<int>
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
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
