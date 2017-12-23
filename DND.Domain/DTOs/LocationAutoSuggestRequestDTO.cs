using DND.Base.Implementation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using DND.Base.Extensions;

namespace DND.Domain.DTOs
{
    public class LocationAutoSuggestRequestDTO : BaseEntity<int>
    {
        public string Locale { get; set; }
        public string[] Market { get; set; }
        public string Currency { get; set; }
        public string Search { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Market == null || Market.Count() == 0)
                yield return new ValidationResult("Markets Required", new string[] { this.Name(x => x.Market) });
            if (string.IsNullOrEmpty(Currency))
                yield return new ValidationResult("Currency Required", new string[] { this.Name(x => x.Currency) });
            if (string.IsNullOrEmpty(Locale))
                yield return new ValidationResult("Locale Required", new string[] { this.Name(x => x.Locale) });
            //if (string.IsNullOrEmpty(Search))
            //    yield return new ValidationResult("Search Required", new string[] { this.Name(x => x.Search) });
        }
    }
}
