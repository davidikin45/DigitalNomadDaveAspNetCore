using DND.Common.Domain.Dtos;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Common.Dtos
{
    public class WebApiPagedRequestDto : DtoBase
    {
        // no. of records to fetch
        public int? PageSize
        { get; set; }

        // the page index
        public int? Page
        { get; set; }

        public string Fields
        { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
