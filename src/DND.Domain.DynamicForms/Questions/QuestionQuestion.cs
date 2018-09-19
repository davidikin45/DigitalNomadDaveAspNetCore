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
    public class QuestionQuestion : EntityAuditableBase<int>
    {
        public int QuestionId { get; set; }

        public int LogicQuestionId { get; set; }
        public Question LogicQuestion { get; set; }

        public string LogicTypeString
        {
            get { return LogicType.ToString(); }
            private set { LogicType = EnumExtensions.ParseEnum<QuestionQuestionLogicType>(value); }
        }

        public QuestionQuestionLogicType LogicType = QuestionQuestionLogicType.ShowAnswerOnceContains;

        public string Value { get; set; }

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
