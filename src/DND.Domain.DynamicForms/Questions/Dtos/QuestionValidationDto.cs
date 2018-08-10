using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Domain.DynamicForms.Sections.Dtos;
using DND.Domain.DynamicForms.Questions.Enums;
using DND.Domain.DynamicForms.Sections;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Dtos
{
    public class QuestionValidationDto : BaseDto<int>, IMapFrom<QuestionValidation>, IMapTo<QuestionValidation>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [HiddenInput()]
        public int QuestionId { get; set; }

        public QuestionValidationType ValidationType { get; set; }

        public string CustomValidationMessage { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {

        }

        public override IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            yield break;
        }
    }
}
