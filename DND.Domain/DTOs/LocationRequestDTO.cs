using Solution.Base.Extensions;
using Solution.Base.Implementation.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DND.Domain.DTOs
{
    public class LocationRequestDTO : BaseEntity<string>
    {
        public string Locale { get; set; }
        public string[] Market { get; set; }
        public string Currency { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Market == null || Market.Count() == 0)
                yield return new ValidationResult("Markets Required", new string[] { this.Name(x => x.Market) });
            if (string.IsNullOrEmpty(Currency))
                yield return new ValidationResult("Currency Required", new string[] { this.Name(x => x.Currency) });
            if (string.IsNullOrEmpty(Locale))
                yield return new ValidationResult("Locale Required", new string[] { this.Name(x => x.Locale) });
            if (string.IsNullOrEmpty(Id))
                yield return new ValidationResult("Id Required", new string[] { this.Name(x => x.Id) });
        }
    }
}
