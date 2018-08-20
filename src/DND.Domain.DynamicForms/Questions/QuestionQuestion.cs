using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Questions.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions
{
    public class QuestionQuestion : BaseEntityAuditable<int>
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
            throw new NotImplementedException();
        }

        public override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            throw new NotImplementedException();
        }
    }
}
