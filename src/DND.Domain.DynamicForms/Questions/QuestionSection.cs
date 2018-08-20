using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions
{
    public class QuestionSection : BaseEntityAuditable<int>
    {
        public int QuestionId { get; set; }

        public string Name { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        public string LogicTypeString
        {
            get { return LogicType.ToString(); }
            private set { LogicType = EnumExtensions.ParseEnum<QuestionSectionLogicType>(value); }
        }

        public QuestionSectionLogicType LogicType = QuestionSectionLogicType.Repeat;

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
