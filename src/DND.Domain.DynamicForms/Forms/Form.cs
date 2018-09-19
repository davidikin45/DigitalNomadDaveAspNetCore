using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using DND.Domain.DynamicForms.Sections;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.Forms
{
    public class Form : EntityAggregateRootAuditableBase<int>
    {
        public bool Published { get; set; }

        public string Name { get; set; }

        public string UrlSlug { get; set; }

        public string ConfirmationText { get; set; }

        public List<FormSection> Sections { get; set; } = new List<FormSection>();

        public List<FormNotification> Notifications { get; set; } = new List<FormNotification>();

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
