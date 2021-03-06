﻿using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Forms.Dtos
{
    public class FormDto : DtoAggregateRootBase<int>, IHaveCustomMappings
    {
        [Required()]
        public string Name { get; set; }

        public string UrlSlug { get; set; }

        [Required()]
        public bool Published { get; set; }

        [MultilineText(HTML = true)]
        public string ConfirmationText { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Sections")]
        [Repeater("{" + nameof(FormSectionDto.Id) + "}")]
        public List<FormSectionDto> Sections { get; set; }

        [Render(ShowForGrid = true, AllowSortForGrid = false, LinkToCollectionInGrid = true, ShowForDisplay = true, ShowForEdit = true)]
        [ActionLink("Notifications")]
        [Repeater("{" + nameof(FormNotificationDto.Email) + "}")]
        public List<FormNotificationDto> Notifications { get; set; }

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
