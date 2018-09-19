﻿using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.DynamicForms.Questions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Sections
{
    public class SectionQuestion : EntityAuditableBase<int>
    {
        public int SectionId { get; set; }
        public Section Section { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
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
