using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Tags;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPostTag : BaseEntityAuditable<int>
    {
        //[Required]
        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }

        //[Required]
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

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}