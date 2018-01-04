using Solution.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Models
{
    public class Testimonial : BaseEntityAuditable<int>
    {
        [Required, StringLength(100)]
        public string Source
        { get; set; }

        [Required, StringLength(5000)]
        public string QuoteText { get; set; }

        public string File { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
