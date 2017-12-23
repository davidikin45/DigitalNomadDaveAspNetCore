﻿using DND.Base.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Models
{
    public class Faq : BaseEntityAuditable<int>
    {
        [Required]
        public string Question { get; set; }

        [Required]
        public string Answer { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
