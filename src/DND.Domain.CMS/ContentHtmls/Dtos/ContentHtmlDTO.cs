using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.CMS.ContentHtmls.Dtos
{
    public class ContentHtmlDto : BaseDtoAggregateRoot<string>, IHaveCustomMappings
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

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<ContentHtmlDto, ContentHtml>()
             .ForMember(bo => bo.DateModified, dto => dto.Ignore())
            .ForMember(bo => bo.DateCreated, dto => dto.Ignore());

            configuration.CreateMap<ContentHtml, ContentHtmlDto>();
        }
    }
}
