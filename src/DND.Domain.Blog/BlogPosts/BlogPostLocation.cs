using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Locations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.Blog.BlogPosts
{
    public class BlogPostLocation : BaseEntityAuditable<int>
    {
        //[Required]
        public int BlogPostId { get; set; }
        //public virtual BlogPost BlogPost { get; set; }

        //[Required]
        public int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public BlogPostLocation()
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