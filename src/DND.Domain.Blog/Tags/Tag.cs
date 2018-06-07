using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.Blog.Tags
{
    public class Tag : BaseEntityAggregateRootAuditable<int>
    {
        //[Required, StringLength(50)]
        public string Name
        { get; set; }

        //[Required, StringLength(50)]
        public string UrlSlug
        { get; set; }

        //[Required, StringLength(200)]
        public string Description
        { get; set; }

        //public virtual IList<BlogPostTag> Posts
        //{ get; set; }

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
