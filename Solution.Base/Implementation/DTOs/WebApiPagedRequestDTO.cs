using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.DTOs
{
    public class WebApiPagedRequestDTO : BaseDTO
    {
        [JsonIgnore]
        public int PageSize
        {
            get { return Rows; }
            set { Rows = value; }
        }

        // no. of records to fetch
        public int Rows
        { get; set; }

        // the page index
        public int Page
        { get; set; }

        [JsonIgnore]
        public string OrderBy
        {
            get { return Sidx; }
            set { Sidx = value; }
        }

        // sort column name
        public string Sidx
        { get; set; }

        [JsonIgnore]
        public string OrderType
        {
            get { return Sord; }
            set { Sord = value; }
        }

        // sort order "asc" or "desc"
        public string Sord
        { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
