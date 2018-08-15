using AutoMapper;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.Automapper;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using DND.Common.ModelMetadataCustom.LinkAttributes;
using DND.Domain.DynamicForms.Questions.Enums;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DND.Domain.DynamicForms.Questions.Dtos
{
    public class QuestionQuestionDto : BaseDto<int>, IMapFrom<QuestionQuestion>, IMapTo<QuestionQuestion>
    {
        [HiddenInput()]
        [ReadOnlyHiddenInput(ShowForCreate = false, ShowForEdit = false), Display(Order = 0)]
        public override int Id { get => base.Id; set => base.Id = value; }

        [ActionLink("Details", "AdminQuestions")]
        [LinkRouteValue("id", "{LogicQuestionId}")]
        [Display(Name = "Question")]
        [Required]
        [Dropdown(typeof(Question), nameof(DND.Domain.DynamicForms.Questions.Question.QuestionText))]
        public int LogicQuestionId { get; set; }

        [HiddenInput()]
        public int QuestionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [Render(ShowForGrid = false, ShowForDisplay = false, ShowForEdit = false, ShowForCreate = false)]
        public QuestionDto LogicQuestion { get; set; }

        public QuestionQuestionLogicType LogicType { get; set; }

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
