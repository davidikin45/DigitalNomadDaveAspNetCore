using DND.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Models
{
    public class Project : BaseEntityAuditable<int>
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
