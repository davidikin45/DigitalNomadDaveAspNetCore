using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.DynamicForms.Forms;
using DND.Domain.DynamicForms.FormSectionSubmissions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.FormSubmissions
{
    public class FormSubmission : EntityAggregateRootAuditableBase<Guid>
    {
        public int FormId { get; set; }
        public Form Form { get; set; }

        public bool Completed { get; set; }
        public bool Valid {get; set; }

        public List<FormSectionSubmission> FormSectionSubmissions { get; set; } = new List<FormSectionSubmission>();

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
