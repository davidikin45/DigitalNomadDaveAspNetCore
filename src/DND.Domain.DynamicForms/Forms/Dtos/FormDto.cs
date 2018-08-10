using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Forms.Dtos
{
    public class FormDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        [Required()]
        public string Name { get; set; }

        [Required()]
        public string UrlSlug { get; set; }

        [Required()]
        public bool Published { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Sections")]
        [Repeater("{" + nameof(FormSectionDto.Id) + "}")]
        public List<FormSectionDto> Sections { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Notifications")]
        [Repeater("{" + nameof(FormNotificationDto.Email) + "}")]
        public List<FormSectionDto> Notifications { get; set; }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Form, FormDto>();

            configuration.CreateMap<FormDto, Form>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }
    }
}
