using DND.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DND.Domain.Models
{
	public class BlogPostTag : BaseEntityAuditable<int>
    {
        [Required]
        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        [Required]
        public int TagId { get; set; }
        public virtual Tag Tag { get; set; }

        public BlogPostTag()
		{
			
		}

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}