﻿using DND.Base.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Models
{
    public class MailingList : BaseEntityAuditable<int>
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
