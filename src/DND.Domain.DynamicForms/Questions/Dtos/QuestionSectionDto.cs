﻿using AutoMapper;
using DND.Common.Domain.Dtos;
using DND.Common.Domain.ModelMetadata;
using DND.Common.Infrastructure.Interfaces.Automapper;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections;
using DND.Domain.DynamicForms.Sections.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Dtos
{
    public class QuestionSectionDto : DtoBase<int>, IMapFrom<QuestionSection>, IMapTo<QuestionSection>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        public string Name { get; set; }

        [ActionLink("Details", "AdminSections")]
        [LinkRouteValue("id", "{SectionId}")]
        [Display(Name = "Section")]
        [Required]
        [Dropdown(typeof(Section), nameof(DND.Domain.DynamicForms.Sections.Section.Name))]
        public int SectionId { get; set; }

        [HiddenInput()]
        public int QuestionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public SectionDto Section { get; set; }

        public QuestionSectionLogicType LogicType { get; set; }

        public string Value { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
