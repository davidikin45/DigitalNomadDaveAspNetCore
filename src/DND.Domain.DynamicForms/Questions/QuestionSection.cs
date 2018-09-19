using DND.Common.Domain;
using DND.Common.Extensions;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Questions
{
    public class QuestionSection : EntityAuditableBase<int>
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
