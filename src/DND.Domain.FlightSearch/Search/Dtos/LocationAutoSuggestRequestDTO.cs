﻿using DND.Common.Domain.Dtos;
using DND.Common.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace DND.Domain.FlightSearch.Search.Dtos
{
    public class LocationAutoSuggestRequestDto : DtoBase<int>
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
