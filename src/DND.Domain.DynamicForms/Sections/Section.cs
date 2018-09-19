using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Sections
{
    public class Section : EntityAggregateRootAuditableBase<int>
    {
        public string Name { get; set; }

        public string UrlSlug { get; set; }

        public Boolean ShowInMenu { get; set; } = true;

        public List<SectionQuestion> Questions { get; set; } = new List<SectionQuestion>();
        public List<SectionSection> Sections { get; set; } = new List<SectionSection>();

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
