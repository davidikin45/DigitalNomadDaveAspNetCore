using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.CMS.Faqs
{
    public class Faq : BaseEntityAggregateRootAuditable<int>
    {
        //[Required]
        public string Question { get; set; }

        //[Required]
        public string Answer { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }

        public async override Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
