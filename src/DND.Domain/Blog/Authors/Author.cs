using DND.Common.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.Blog.Authors
{
    public class Author : BaseEntityAggregateRootAuditable<int>
    {
        //[Required]
		public string Name { get; set; }

        //[StringLength(50)]
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