using DND.Common.Enums;
using DND.Common.Implementation.Models;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace DND.Domain.DynamicForms.LookupTables
{
    public class LookupTable : BaseEntityAggregateRootAuditable<int>
    {
        public string Name { get; set; }

        public List<LookupTableItem> LookupTableItems { get; set; } = new List<LookupTableItem>();

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }

        public override async Task<IEnumerable<ValidationResult>> ValidateWithDbConnectionAsync(IBaseUnitOfWorkScope unitOfWork, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();
            return errors;
        }
    }
}
