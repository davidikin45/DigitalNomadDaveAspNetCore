using DND.Base.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Models
{
    public class ContentHtml : BaseEntityAuditable<string>
    {

        public string HTML { get; set; }

        public bool PreventDelete { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
