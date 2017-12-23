using DND.Base.Implementation.Models;
using DND.Base.Interfaces.Automapper;
using DND.Base.ModelMetadata;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DND.Domain.Models;

namespace DND.Domain.DTOs
{
    public class ContentHtmlDTO : BaseEntity<string>, IMapTo<ContentHtml>, IMapFrom<ContentHtml>
    {
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = true)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [MultilineText(HTML = true, Rows = 7)]
        public string HTML { get; set; }
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
