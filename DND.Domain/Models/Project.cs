using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Models
{
    public class Project : BaseEntityAggregateRootAuditable<int>
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        public string Link { get; set; }

        public string File { get; set; }

        public string Album { get; set; }

        [Required, StringLength(200)]
        public string DescriptionText { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
