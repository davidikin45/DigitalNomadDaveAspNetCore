using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Implementation.Dtos
{
    public class WebApiPagedRequestDto : BaseDto
    {
        // no. of records to fetch
        public int PageSize
        { get; set; }

        // the page index
        public int Page
        { get; set; }

        // sort column name
        public string OrderBy
        { get; set; }

        // sort order "asc" or "desc"
        public string OrderType
        { get; set; }

        public string Search
        { get; set; }

        public string Fields
        { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
