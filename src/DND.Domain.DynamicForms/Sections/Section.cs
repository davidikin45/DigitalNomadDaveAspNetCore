using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.DynamicForms.Sections.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Sections
{
    public class Section : BaseEntityAggregateRootAuditable<int>
    {
        public string Name { get; set; }

        public string UrlSlug { get; set; }

        public string SectionTypeString
        {
            get { return SectionType.ToString(); }
            private set { SectionType = EnumExtensions.ParseEnum<SectionType>(value); }
        }

        public SectionType SectionType = SectionType.Questions;

        public Boolean ShowInMenu { get; set; } = true;

        public List<SectionQuestion> Questions { get; set; } = new List<SectionQuestion>();

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
