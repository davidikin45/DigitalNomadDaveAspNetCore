using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentHtmls
{
    public class ContentHtml : BaseEntityAggregateRootAuditable<string>
    {

        public string HTML { get; set; }

        public bool PreventDelete { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
