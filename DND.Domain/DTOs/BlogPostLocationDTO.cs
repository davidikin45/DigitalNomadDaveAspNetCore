using DND.Domain.Models;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DTOs
{
    public class BlogPostLocationDTO : BaseDTO, IMapFrom<Location>, IMapTo<BlogPostLocation>
    {
        [Required]
        public int LocationId { get; set; }

        public int BlogPostId { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
