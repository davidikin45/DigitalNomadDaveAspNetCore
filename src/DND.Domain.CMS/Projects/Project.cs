using DND.Common.Domain;
using DND.Common.Infrastrucutre.Interfaces.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.CMS.Projects
{
    public class Project : EntityAggregateRootAuditableBase<int>
    {
        //[Required, StringLength(100)]
        public string Name { get; set; }

        public string Link { get; set; }

        public string File { get; set; }

        public string Album { get; set; }

        //[Required, StringLength(200)]
        public string DescriptionText { get; set; }

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
