using DND.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Domain.Models
{
    public class Tag : BaseEntityAuditable<int>
    {
        [Required, StringLength(50)]
        public string Name
        { get; set; }

        [Required, StringLength(50)]
        public string UrlSlug
        { get; set; }

        [Required, StringLength(200)]
        public string Description
        { get; set; }

        //public virtual IList<BlogPostTag> Posts
        //{ get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
