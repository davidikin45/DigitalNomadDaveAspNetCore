using DND.Domain.Constants;
using DND.Domain.Enums;
using DND.Domain.Models;
using DND.Base.Implementation.Models;
using DND.Base.Interfaces.Automapper;
using DND.Base.ModelMetadata;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
	public class AuthorDTO : BaseEntity<int> , IMapFrom<Author>, IMapTo<Author>
    {
        [Required]
		public string Name { get; set; }

        [StringLength(50)]
        public string UrlSlug { get; set; }

        public AuthorDTO()
		{

        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}