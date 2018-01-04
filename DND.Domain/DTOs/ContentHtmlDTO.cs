using DND.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Implementation.Models;
using Solution.Base.Interfaces.Automapper;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
