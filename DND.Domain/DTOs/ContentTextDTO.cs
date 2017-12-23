using DND.Base.Implementation.Models;
using DND.Base.Interfaces.Automapper;
using DND.Base.ModelMetadata;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace DND.Domain.DTOs
{
    public class ContentTextDTO : BaseEntity<string>, IMapTo<ContentText>, IMapFrom<ContentText>
    {
        [MultilineText(HTML = false, Rows = 7)]
        public string Text { get; set; }
        //[DataType(DataType.Date)]
        //public DateTime FromDate { get; set; }
        //[DataType(DataType.Date)]
        //public DateTime? ToDate { get; set; }
        //public DbGeography Location { get; set; }

        [HiddenInput]
        public bool PreventDelete { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            yield break;
        }
    }
}
