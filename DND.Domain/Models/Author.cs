using DND.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Domain.Models
{
	public class Author : BaseEntityAuditable<int>
    {
        [Required]
		public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        public Author()
		{
			
		}

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}