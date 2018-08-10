﻿using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Domain.DynamicForms.Sections.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Sections.Dtos
{
    public class SectionDto : BaseDtoAggregateRoot<int>, IHaveCustomMappings
    {
        [Required()]
        public string Name { get; set; }

        [Required()]
        public string UrlSlug { get; set; }

        public SectionType SectionType { get; set; }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Section, SectionDto>();

            configuration.CreateMap<SectionDto, Section>()
                .ForMember(bo => bo.DateModified, dto => dto.Ignore())
                 .ForMember(bo => bo.DateCreated, dto => dto.Ignore());
        }
    }
}
