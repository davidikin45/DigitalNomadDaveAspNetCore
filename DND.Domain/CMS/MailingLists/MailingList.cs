using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.MailingLists
{
    public class MailingList : BaseEntityAggregateRootAuditable<int>
    {
        public string Name { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
