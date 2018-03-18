﻿using DND.Domain.Models;
using DND.Common.Implementation.DTOs;
using DND.Common.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class BlogPostTagDTO : BaseDTO, IMapFrom<Tag>, IMapTo<BlogPostTag>
    {
        [Required]
        public int TagId { get; set; }

        public int BlogPostId { get; set; }

        public string Name { get; set; }
        public string UrlSlug { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
